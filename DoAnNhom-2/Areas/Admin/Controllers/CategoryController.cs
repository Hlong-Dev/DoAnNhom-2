
using DoAnNhom_2.Data;
using DoAnNhom_2.Models;
using DoAnNhom_2.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DoAnNhom_2.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext _dataContext;
        public CategoryController(ApplicationDbContext context)
        {
            _dataContext = context;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _dataContext.Categories.OrderByDescending(p => p.Id).Where(p => p.IsDeleted == false).ToListAsync());
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CategoryModel category)
        {
            if (ModelState.IsValid)
            {
                // Check if category with the same name already exists
                category.Slug = category.Name.Replace(" ", "-");
                var slug = await _dataContext.Categories.FirstOrDefaultAsync(p => p.Slug == category.Slug);
                if (slug != null)
                {
                    ModelState.AddModelError("", "Category already exists.");
                    return View(category);
                }

                _dataContext.Add(category);
                await _dataContext.SaveChangesAsync();
                TempData["success"] = "Thêm danh mục thành công";
                return RedirectToAction("Index");
            }
            else
            {
                TempData["error"] = "Model có một vài thứ đang bị lỗi";
                List<string> errors = new List<string>();
                foreach (var value in ModelState.Values)
                {
                    foreach (var error in value.Errors)
                    {
                        errors.Add(error.ErrorMessage);
                    }
                }
                string errorMessage = string.Join("\n", errors);
                return BadRequest(errorMessage);
            }
        }

        public async Task<IActionResult> Edit(int Id)
        {
            CategoryModel category = await _dataContext.Categories.FindAsync(Id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int Id, CategoryModel category)
        {
            if (Id != category.Id)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                // Check if category with the same name already exists
                category.Slug = category.Name.Replace(" ", "-");
                var slug = await _dataContext.Categories.FirstOrDefaultAsync(p => p.Slug == category.Slug && p.Id != category.Id);
                if (slug != null)
                {
                    ModelState.AddModelError("", "Category already exists.");
                    return View(category);
                }

                _dataContext.Update(category);
                await _dataContext.SaveChangesAsync();
                TempData["success"] = "Cập nhật danh mục thành công";
                return RedirectToAction("Index");
            }
            else
            {
                TempData["error"] = "Model có một vài thứ đang bị lỗi";
                List<string> errors = new List<string>();
                foreach (var value in ModelState.Values)
                {
                    foreach (var error in value.Errors)
                    {
                        errors.Add(error.ErrorMessage);
                    }
                }
                string errorMessage = string.Join("\n", errors);
                return BadRequest(errorMessage);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int Id)
        {
            // Find the category by Id
            CategoryModel category = await _dataContext.Categories.FindAsync(Id);
            if (category == null)
            {
                return NotFound();
            }

            // Check if there are any products associated with this category
            var productsInCategory = await _dataContext.Products.AnyAsync(p => p.CategoryId == Id);
            if (productsInCategory)
            {
                // If there are products associated with this category, return an error message
                return Json(new { error = "Không thể xóa danh mục vì còn sản phẩm trong danh mục này." });
            }

            // Set IsDeleted to true
            category.IsDeleted = true;

            // Update the category in the database
            _dataContext.Update(category);
            await _dataContext.SaveChangesAsync();

            // Return success message
            return Json(new { success = "Danh mục đã được đánh dấu là đã xóa" });
        }

       


    }
}
