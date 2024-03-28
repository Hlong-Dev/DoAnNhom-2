using DoAnNhom_2.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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

        public IActionResult Index()
        {
            var product = _dataContext.Products.OrderByDescending(p => p.Id).Include(p => p.Category).Include(p => p.Brand).Where(p => p.IsDeleted == false).ToList();
            return View(product);
        }

        public IActionResult Details(int id)
        {
             var product = _dataContext.Products.Include(p => p.Category).Include(p => p.Brand).FirstOrDefault(p => p.Id == id  && p.IsDeleted == false);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }
        [HttpPost]
        public IActionResult Search(string searchTerm)
        {
            var results = _dataContext.Products
                .Include(p => p.Category)
                .Include(p => p.Brand)
                .Where(p => p.IsDeleted == false && (p.Name.Contains(searchTerm) || p.Description.Contains(searchTerm)))
                .ToList();

            return View("SearchResult", results);
        }
    }
}
