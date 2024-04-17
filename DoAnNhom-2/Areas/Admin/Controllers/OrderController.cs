using DoAnNhom_2.Models;
using DoAnNhom_2.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using OfficeOpenXml.Style;
using OfficeOpenXml;
using System.Drawing;

namespace DoAnNhom_2.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
    public class OrderController : Controller
    {
        private readonly ApplicationDbContext _dataContext;

        public OrderController(ApplicationDbContext context)
        {
            _dataContext = context;
        }

        [Route("quan-ly-don-hang")]
        public async Task<IActionResult> Index()
        {
            var orders = await _dataContext.Orders.OrderByDescending(p => p.Id).ToListAsync();
            int totalOrders = orders.Count();
            ViewBag.TotalOrders = totalOrders;
            return View(orders);
        }

        // GET: Admin/Order/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _dataContext.OrderDetails.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // POST: Admin/Order/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var order = await _dataContext.OrderDetails.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }

            _dataContext.OrderDetails.Remove(order);
            await _dataContext.SaveChangesAsync();

            TempData["Message"] = "Đã xóa đơn hàng thành công."; // Lưu thông báo vào TempData

            // Redirect to the specified URL after successful deletion
            return RedirectToAction("Index", "Order", new { area = "Admin" });
            // Redirect to /quan-ly-don-hang
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
