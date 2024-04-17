using DoAnNhom_2.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DoAnNhom_2.Repository;
using Microsoft.AspNetCore.Authorization;
using DoAnNhom_2.Data;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Drawing;


namespace DoAnNhom_2.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _dataContext;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _dataContext = context;
            _webHostEnvironment = webHostEnvironment;
        }
        [Route("quan-ly-san-pham")]
        public async Task<IActionResult> Index()
        {
            return View(await _dataContext.Products.OrderByDescending(p => p.Id).Include(p => p.Category).Include(p => p.Brand).Where(p => p.IsDeleted == false).ToListAsync());
        }
        [Authorize(Roles = SD.Role_Admin)]
        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.Categories = new SelectList(_dataContext.Categories, "Id", "Name");
            ViewBag.Brands = new SelectList(_dataContext.Brands, "Id", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductModel product)
        {
            ViewBag.Categories = new SelectList(_dataContext.Categories, "Id", "Name", product.CategoryId);
            ViewBag.Brands = new SelectList(_dataContext.Brands, "Id", "Name", product.BrandId);
            if (ModelState.IsValid)
            {
                product.Slug = product.Name.Replace(" ", "-");
                var slug = await _dataContext.Products.FirstOrDefaultAsync(p => p.Slug == product.Slug);
                if (slug != null)
                {
                    ModelState.AddModelError("", "Sản Phẩm đã có trong db ");
                    return View(product);
                }

                if (product.ImageUpload != null)
                {
                    string uploadsDir = Path.Combine(_webHostEnvironment.WebRootPath, "media/products");
                    string imageName = Guid.NewGuid().ToString() + "_" + product.ImageUpload.FileName;
                    string filePath = Path.Combine(uploadsDir, imageName);

                    using (FileStream fs = new FileStream(filePath, FileMode.Create))
                    {
                        await product.ImageUpload.CopyToAsync(fs);
                    }
                    product.Image = imageName;
                }

                _dataContext.Add(product);
                await _dataContext.SaveChangesAsync();
                TempData["success"] = "Thêm sản phẩm thành công";
                return RedirectToAction("Index");
            }
            else
            {
                TempData["error"] = "Model có một vài thứ đang bị lỗi";
                List<string> errors = new List<string>();
                foreach (var value in ModelState.Values)
                {
                    foreach (var error in value.Errors)
                    {
                        errors.Add(error.ErrorMessage);
                    }
                }
                string errorMessage = string.Join("\n", errors);
                return BadRequest(errorMessage);
            }
        }
        [Authorize(Roles = SD.Role_Admin)]
        [HttpGet]
        public async Task<IActionResult> Edit(int Id)
        {
            ProductModel product = await _dataContext.Products.FindAsync(Id);
            if (product == null)
            {
                return NotFound();
            }

            ViewBag.Categories = new SelectList(_dataContext.Categories, "Id", "Name", product.CategoryId);
            ViewBag.Brands = new SelectList(_dataContext.Brands, "Id", "Name", product.BrandId);
            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int Id, ProductModel product)
        {
            ViewBag.Categories = new SelectList(_dataContext.Categories, "Id", "Name", product.CategoryId);
            ViewBag.Brands = new SelectList(_dataContext.Brands, "Id", "Name", product.BrandId);
            if (Id != product.Id)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                product.Slug = product.Name.Replace(" ", "-");
                var slug = await _dataContext.Products.FirstOrDefaultAsync(p => p.Slug == product.Slug && p.Id != product.Id);
                if (slug != null)
                {
                    ModelState.AddModelError("", "Sản Phẩm đã có trong db ");
                    return View(product);
                }

                if (product.ImageUpload != null)
                {
                    string uploadDir = Path.Combine(_webHostEnvironment.WebRootPath, "media/products");
                    string imageName = Guid.NewGuid().ToString() + "_" + product.ImageUpload.FileName;
                    string filePath = Path.Combine(uploadDir, imageName);

                    using (FileStream fs = new FileStream(filePath, FileMode.Create))
                    {
                        await product.ImageUpload.CopyToAsync(fs);
                    }
                    product.Image = imageName;
                }

                // Lấy giá cũ từ database
                var oldProduct = await _dataContext.Products.AsNoTracking().FirstOrDefaultAsync(p => p.Id == Id);

                // Kiểm tra nếu giá mới khác giá cũ, cập nhật giá cũ và giá mới
                if (product.Price != oldProduct.Price)
                {
                    product.OldPrice = oldProduct.Price; // Lưu giá cũ
                }
                else
                {
                    product.OldPrice = oldProduct.OldPrice; // Giữ nguyên giá cũ nếu giá mới không thay đổi
                }

                try
                {
                    _dataContext.Update(product);
                    await _dataContext.SaveChangesAsync();
                    TempData["success"] = "Cập nhật sản phẩm thành công";
                    return RedirectToAction("Index");
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            else
            {
                TempData["error"] = "Model có một vài thứ đang bị lỗi";
                List<string> errors = new List<string>();
                foreach (var value in ModelState.Values)
                {
                    foreach (var error in value.Errors)
                    {
                        errors.Add(error.ErrorMessage);
                    }
                }
                string errorMessage = string.Join("\n", errors);
                return BadRequest(errorMessage);
            }
        }

        [Authorize(Roles = SD.Role_Admin)]
        public async Task<IActionResult> Delete(int Id)
        {
            ProductModel product = await _dataContext.Products.FindAsync(Id);
            if (product != null)
            {
                if (!string.Equals(product.Image, "noname.jpg"))
                {
                    string uploadDir = Path.Combine(_webHostEnvironment.WebRootPath, "media/products");

                    string oldfileImage = Path.Combine(uploadDir, product.Image);
                    if (System.IO.File.Exists(oldfileImage))
                    {
                        System.IO.File.Delete(oldfileImage);
                    }
                }
                product.IsDeleted = true;
                _dataContext.Update(product);
                await _dataContext.SaveChangesAsync();
                TempData["error"] = "Sản phẩm đã xóa";
                return RedirectToAction("Index");
            }
            else
            {
                return NotFound();
            }
        }

        private bool ProductExists(int id)
        {
            return _dataContext.Products.Any(e => e.Id == id);
        }
        [Authorize(Roles = SD.Role_Admin)]
        [HttpPost]
        public async Task<IActionResult> Import(IFormFile file)
        {
            if (file == null || file.Length <= 0)
            {
                TempData["error"] = "Vui lòng chọn một tệp để tải lên.";
                return RedirectToAction("Index");
            }

            using (var stream = new MemoryStream())
            {
                await file.CopyToAsync(stream);
                using (var package = new ExcelPackage(stream))
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                    int rowCount = worksheet.Dimension.Rows;

                    for (int row = 2; row <= rowCount; row++)
                    {
                        var productName = worksheet.Cells[row, 1].Value.ToString().Trim();
                        var categoryId = int.Parse(worksheet.Cells[row, 2].Value.ToString());
                        var brandId = int.Parse(worksheet.Cells[row, 3].Value.ToString());
                        var price = decimal.Parse(worksheet.Cells[row, 4].Value.ToString());
                        var Description = worksheet.Cells[row, 5].Value.ToString();
                        
                        // Tạo đối tượng ProductModel từ dữ liệu tệp Excel
                        var product = new ProductModel
                        {
                            Name = productName,
                            CategoryId = categoryId,
                            BrandId = brandId,
                            Price = price,
                            IsDeleted = false, // Bạn có thể thay đổi giá trị này tùy theo yêu cầu của mình
                            Description = Description,
                        };

                        // Thêm sản phẩm vào cơ sở dữ liệu
                        _dataContext.Add(product);
                    }

                    await _dataContext.SaveChangesAsync();
                }
            }

            TempData["success"] = "Nhập sản phẩm từ tệp Excel thành công.";
            return RedirectToAction("Index");
        }

        [Authorize(Roles = SD.Role_Admin)]
        public async Task<IActionResult> Export()
        {
            var products = await _dataContext.Products.ToListAsync();

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Sản phẩm");

                // Thiết lập tiêu đề cho các cột
                worksheet.Cells[1, 1].Value = "Tên Sản phẩm";
                worksheet.Cells[1, 2].Value = "Số Lượng";
                worksheet.Cells[1, 3].Value = "Giá Cũ";
                worksheet.Cells[1, 4].Value = "Giá";
                worksheet.Cells[1, 5].Value = "Kích Thước";

                worksheet.Cells[1, 6].Value = "Thương Hiệu";
                // Ghi dữ liệu sản phẩm vào tệp Excel
                int row = 2;
                foreach (var product in products)
                {
                    worksheet.Cells[row, 1].Value = product.Name;
                    worksheet.Cells[row, 2].Value = product.Quantity;
                    worksheet.Cells[row, 3].Value = product.OldPrice;
                    worksheet.Cells[row, 4].Value = product.Price;
                    worksheet.Cells[row, 5].Value = product.Size;
                    worksheet.Cells[row, 5].Value = product.BrandId;

                    row++;
                }

                // Thiết lập kiểu dáng cho bảng
                worksheet.Cells["A1:D1"].Style.Font.Bold = true;
                worksheet.Cells["A1:D1"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells["A1:D1"].Style.Fill.BackgroundColor.SetColor(Color.LightGray);

                // AutoFit cột
                worksheet.Cells.AutoFitColumns();

                // Xuất tệp Excel
                return File(package.GetAsByteArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "products.xlsx");
            }
        }
    }
}
