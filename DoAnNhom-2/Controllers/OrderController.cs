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
        
        [Authorize] 
        public async Task<IActionResult> Index()
        {
         
            var userId = User.Identity.Name; 
            var orders = await _context.OrderDetails
                .Where(od => od.UserName == userId)
                .ToListAsync();

            return View(orders);
        }
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.OrderDetails.FindAsync(id);
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
            var order = await _context.OrderDetails.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }

            _context.OrderDetails.Remove(order);
            await _context.SaveChangesAsync();

            TempData["Message"] = "Đã xóa đơn hàng thành công.";

       
            return RedirectToAction("Index", "Order", new { area = "" });
        
        }
    }
}
