using DoAnNhom_2.Data;
using DoAnNhom_2.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace DoAnNhom_2.Controllers
{
    public class OrderController : Controller
    {
        private readonly ApplicationDbContext _context;

        public OrderController(ApplicationDbContext context)
        {
            _context = context;
        }
        [Route("don-hang-da-mua")]
        // Action để hiển thị các đơn hàng của người dùng
        [Authorize] // Đảm bảo chỉ người dùng đăng nhập mới có thể truy cập
        public async Task<IActionResult> Index()
        {
            // Lấy danh sách các đơn hàng của người dùng hiện tại
            var userId = User.Identity.Name; // Lấy ID của người dùng đăng nhập hiện tại
            var orders = await _context.OrderDetails
                .Where(od => od.UserName == userId)
                .ToListAsync();

            return View(orders);
        }
    }
}
