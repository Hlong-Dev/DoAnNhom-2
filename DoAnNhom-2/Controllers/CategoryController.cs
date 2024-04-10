using DoAnNhom_2.Data;
using DoAnNhom_2.Models;
using DoAnNhom_2.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DoAnNhom2.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext _dataContext;
        public CategoryController(ApplicationDbContext context) 
        {
            _dataContext = context;
        }
      
        public async Task<IActionResult> Index(string Slug = "")
        {
            CategoryModel categoty = _dataContext.Categories.Where(c => c.Slug == Slug).FirstOrDefault();
            if (categoty == null) return RedirectToAction("Index");
            var productsByCategory = _dataContext.Products.Where(c => c.CategoryId == categoty.Id);
            return View(await productsByCategory.OrderByDescending(p => p.Id).Where(p => p.IsDeleted == false).ToListAsync());
        }
    }
}
