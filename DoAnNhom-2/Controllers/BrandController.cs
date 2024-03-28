using DoAnNhom_2.Data;
using DoAnNhom_2.Models;
using DoAnNhom_2.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DoAnNhom_2.Controllers
{
    public class BrandController : Controller
    {
        private readonly ApplicationDbContext _dataContext;
        public BrandController(ApplicationDbContext context)
        {
            _dataContext = context;
        }
        public async Task<IActionResult> Index(string Slug = "")
        {
            BrandModel brand = _dataContext.Brands.Where(c => c.Slug == Slug).FirstOrDefault();
            if ( brand == null) return RedirectToAction("Index");
            var productsByBrand = _dataContext.Products.Where(c => c.BrandId == brand.Id);
            return View(await productsByBrand.OrderByDescending(p => p.Id).Where(p => p.IsDeleted == false).ToListAsync());
        }
    }
}
