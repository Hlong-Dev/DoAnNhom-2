//using DoAnNhom_2.Data;
//using DoAnNhom_2.Models;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;

//namespace DoAnNhom_2.Controllers
//{
//    public class DetailController : Controller
//    {
      
//        private readonly ApplicationDbContext _dataContext;

//        public DetailController(ApplicationDbContext context)
//        {
         
//            _dataContext = context;
//        }


//        [Route("{slug}")]
//        public IActionResult Details(string slug)
//        {
//            if (string.IsNullOrEmpty(slug))
//            {
//                return BadRequest(); // Xử lý trường hợp khi không có slug được cung cấp
//            }

//            var h = _dataContext.Products
//                .Include(p => p.Category)
//                .Include(p => p.Brand)
//                .FirstOrDefault(p => p.Slug == slug && !p.IsDeleted);

//            if (h == null)
//            {
//                return NotFound(); // Trả về lỗi 404 nếu không tìm thấy sản phẩm với slug tương ứng
//            }

//            return View(h);
//        }
//    }
//}