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
                        var FullName = worksheet.Cells[row, 1].Value.ToString().Trim(); // Lấy giá trị từ ô thứ nhất
                        var Username = worksheet.Cells[row, 2].Value.ToString().Trim(); // Lấy giá trị từ ô thứ hai
                        var CreateDate = DateTime.Parse(worksheet.Cells[row, 3].Value.ToString());
                        var Status = int.Parse(worksheet.Cells[row, 4].Value.ToString());
                        var Ordercode = Guid.NewGuid().ToString();
                        // Tạo đối tượng Order từ dữ liệu tệp Excel
                        var order = new OrderModel
                        {
                            FullName = FullName,
                            UserName = Username,
                            CreatedDate = CreateDate,
                            Status = Status,
                            Ordercode = Ordercode,
                        };

                        // Thêm đơn hàng vào cơ sở dữ liệu
                        _dataContext.Orders.Add(order);
                    }

                    await _dataContext.SaveChangesAsync();
                }
            }

            TempData["success"] = "Nhập đơn hàng từ tệp Excel thành công.";
            return RedirectToAction("Index", "Order", new { area = "Admin" });
        }



        [Authorize(Roles = SD.Role_Admin)]
        public async Task<IActionResult> Export()
        {
            var orders = await _dataContext.Orders.ToListAsync();

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Đơn hàng");

                // Thiết lập tiêu đề cho các cột
                worksheet.Cells[1, 1].Value = "Id";
                worksheet.Cells[1, 2].Value = "Tên Khách Hàng";
                worksheet.Cells[1, 3].Value = "OrderCode";
                worksheet.Cells[1, 4].Value = "Ngày Tạo";
                worksheet.Cells[1, 5].Value = "Trạng thái đơn hàng";

                int row = 2;
                foreach (var order in orders)
                {
                    worksheet.Cells[row, 1].Value = order.Id;
                    worksheet.Cells[row, 2].Value = order.UserName;
                    worksheet.Cells[row, 3].Value = order.Ordercode;
                    worksheet.Cells[row, 4].Value = order.CreatedDate;
                    worksheet.Cells[row, 5].Value = order.Status;

                    row++;
                }

                // Thiết lập kiểu dáng cho bảng
                worksheet.Cells["A1:E1"].Style.Font.Bold = true;
                worksheet.Cells["A1:E1"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells["A1:E1"].Style.Fill.BackgroundColor.SetColor(Color.LightGray);

                // AutoFit cột
                worksheet.Cells.AutoFitColumns();

                // Xuất tệp Excel
                return File(package.GetAsByteArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "orders.xlsx");
            }
        }



    }
}
