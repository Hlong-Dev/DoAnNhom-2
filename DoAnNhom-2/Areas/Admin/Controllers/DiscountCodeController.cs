using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DoAnNhom_2.Data;
using DoAnNhom_2.Models;
using Microsoft.EntityFrameworkCore;

namespace DoAnNhom_2.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class DiscountCodeController : Controller
    {
        private readonly ApplicationDbContext _dbContext;

        public DiscountCodeController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        [Route("ma-giam-gia-admin")]
        // GET: Admin/DiscountCode
        public async Task<IActionResult> Index1()
        {
            var discountCodes = await _dbContext.DiscountCodes.ToListAsync();
            return View(discountCodes);
        }

        // GET: Admin/DiscountCode/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/DiscountCode/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DiscountCodeModel discountCodeModel)
        {
            if (ModelState.IsValid)
            {
                _dbContext.Add(discountCodeModel);
                await _dbContext.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(discountCodeModel);
        }

        // GET: Admin/DiscountCode/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var discountCodeModel = await _dbContext.DiscountCodes.FindAsync(id);
            if (discountCodeModel == null)
            {
                return NotFound();
            }
            return View(discountCodeModel);
        }

        // POST: Admin/DiscountCode/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, DiscountCodeModel discountCodeModel)
        {
            if (id != discountCodeModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _dbContext.Update(discountCodeModel);
                    await _dbContext.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DiscountCodeModelExists(discountCodeModel.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(discountCodeModel);
        }

        // GET: Admin/DiscountCode/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var discountCodeModel = await _dbContext.DiscountCodes
                .FirstOrDefaultAsync(m => m.Id == id);
            if (discountCodeModel == null)
            {
                return NotFound();
            }

            return View(discountCodeModel);
        }

        // POST: Admin/DiscountCode/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var discountCodeModel = await _dbContext.DiscountCodes.FindAsync(id);
            _dbContext.DiscountCodes.Remove(discountCodeModel);
            await _dbContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DiscountCodeModelExists(int id)
        {
            return _dbContext.DiscountCodes.Any(e => e.Id == id);
        }
    }
}
