using DoAnNhom_2.Models;
using DoAnNhom_2.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

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
    }
}
