using DoAnNhom_2.Data;
using DoAnNhom_2.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using X.PagedList;
namespace DoAnNhom_2.Controllers
{
    public class ProductController : Controller
    {
        private readonly ILogger<ProductController> _logger;
        private readonly ApplicationDbContext _dataContext;

        public ProductController(ILogger<ProductController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _dataContext = context;
        }

        // Action để hiển thị danh sách sản phẩm
        [Route("san-pham")]
        public IActionResult Index(int? page)
        {
            int pageNumber = page ?? 1; // Số trang hiện tại, nếu không có thì mặc định là 1
            int pageSize = 3; // Số sản phẩm trên mỗi trang

            var products = _dataContext.Products
                .OrderByDescending(p => p.Slug)
                .Include(p => p.Category)
                .Include(p => p.Brand)
                .Where(p => !p.IsDeleted)
                .ToPagedList(pageNumber, pageSize);

            return View(products);
        }

        // Action để hiển thị chi tiết sản phẩm
        [Route("{slug}")]
        public IActionResult Details(string slug)
        {
            if (string.IsNullOrEmpty(slug))
            {
                return BadRequest(); // Xử lý trường hợp khi không có slug được cung cấp
            }

            var product = _dataContext.Products
                .Include(p => p.Category)
                .Include(p => p.Brand)
                .FirstOrDefault(p => p.Slug == slug && !p.IsDeleted);

            if (product == null)
            {
                return NotFound(); // Trả về lỗi 404 nếu không tìm thấy sản phẩm với slug tương ứng
            }

            return View(product);
        }

        [HttpPost]
        public IActionResult Search(string searchTerm, int? page)
        {
            int pageNumber = page ?? 1; // Số trang hiện tại, nếu không có thì mặc định là 1
            int pageSize = 3; // Số sản phẩm trên mỗi trang
            var results = _dataContext.Products
                .Include(p => p.Category)
                .Include(p => p.Brand) 
                .Where(p => p.IsDeleted == false && (p.Name.Contains(searchTerm) || p.Description.Contains(searchTerm)))
                .ToPagedList(pageNumber, pageSize);

            return View("SearchResult", results);
        }
        [HttpGet]
        public IActionResult Autocomplete(string term)
        {
            var results = _dataContext.Products
                                .Where(p => p.Name.Contains(term))
                                .Select(p => p.Name)
                                .ToList();

            return Json(results);
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
