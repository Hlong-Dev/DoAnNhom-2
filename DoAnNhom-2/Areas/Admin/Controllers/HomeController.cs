using DoAnNhom_2.Data;
using DoAnNhom_2.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace DoAnNhom_2.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _dataContext;

        public HomeController(ApplicationDbContext context)
        {
            _dataContext = context;
        }

        [Route("quan-ly-admin")]
        public async Task<IActionResult> Index()
        {
            var orders = await _dataContext.OrderDetails.OrderByDescending(p => p.Id).ToListAsync();
            int totalOrders = orders.Count();
            decimal totalAmount = orders.Sum(order => order.Price);
            ViewBag.TotalOrders = totalOrders;
            ViewBag.TotalAmount = totalAmount;

            // Tính tổng thu nhập theo tháng
            var monthlyEarnings = new decimal[12];
            foreach (var order in orders)
            {
                var month = order.CreatedDate.Month;
                monthlyEarnings[month - 1] += order.Price;
            }

            ViewBag.Earnings = string.Join(",", monthlyEarnings);

            return View(orders);
        }
    }
}
