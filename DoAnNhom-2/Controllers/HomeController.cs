using DoAnNhom_2.Data;
using DoAnNhom_2.Models;
using DoAnNhom_2.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace DoAnNhom_2.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _dataContext;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _dataContext = context;
        }

        public IActionResult Index()
        {
            List<CartItemModel> cartitems = HttpContext.Session.GetJson<List<CartItemModel>>("Cart") ?? new List<CartItemModel>();

            var products = _dataContext.Products
                                         .OrderByDescending(p => p.Id)
                                         .Include(p => p.Category)
                                         .Include(p => p.Brand)
                                         .Where(p => p.IsDeleted == false && p.OldPrice > 0) // Chỉ lấy ra sản phẩm có OldPrice > 0
                                         .Take(8) // Giới hạn số lượng sản phẩm lấy ra
                                         .ToList();

            ViewBag.CartItemCount = GetCartItemCount();
            return View(products);
        }

        private int GetCartItemCount()
        {
            List<CartItemModel> cart = HttpContext.Session.GetJson<List<CartItemModel>>("Cart") ?? new List<CartItemModel>();
            return cart.Sum(item => item.Quantity);
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        [HttpGet]
        public IActionResult CheckQuantity(int id)
        {
            var product = _dataContext.Products.FirstOrDefault(p => p.Id == id);
            if (product != null && product.Quantity > 0)
            {
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }
    }
}