//using DoAnNhom_2.Data;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using System.Linq;

//namespace DoAnNhom_2.Controllers
//{
//    public class SearchController : Controller
//    {
//        private readonly ApplicationDbContext _dataContext;

//        public SearchController(ApplicationDbContext context)
//        {
//            _dataContext = context;
//        }

//        public IActionResult Index()
//        {
//            return View();
//        }

//        [HttpGet]
//        public IActionResult Autocomplete(string term)
//        {
//            if (string.IsNullOrEmpty(term))
//            {
//                return BadRequest("Search term is required.");
//            }

//            var results = _dataContext.Products
//                                    .Where(p => p.Name.Contains(term))
//                                    .Select(p => p.Name)
//                                    .ToList();

//            return Json(results);
//        }

//        [HttpPost]
//        public IActionResult Search(string searchTerm)
//        {
//            if (string.IsNullOrEmpty(searchTerm))
//            {
//                // Xử lý yêu cầu không hợp lệ
//                return View("Index", new List<Product>()); // Trả về view với danh sách sản phẩm trống
//            }

//            var results = _dataContext.Products
//                                    .Where(p => p.Name.Contains(searchTerm))
//                                    .ToList();

//            return View("Index", results); // Trả về view với danh sách sản phẩm kết quả
//        }
//    }
//}
