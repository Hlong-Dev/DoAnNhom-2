
using Microsoft.AspNetCore.Mvc;
using DoAnNhom_2.Data;

using Microsoft.EntityFrameworkCore;
using DoAnNhom_2.Models;
using Microsoft.AspNetCore.Authorization;

namespace DoAnNhom_2.Controllers
{
    [Authorize]
    public class DiscountCodeController : Controller
    {
        private readonly ApplicationDbContext _dbContext;

        public DiscountCodeController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        [Route("ma-giam-gia")]
        public async Task<IActionResult> Index()
        {
            var discountCodes = await _dbContext.DiscountCodes.ToListAsync();
            return View(discountCodes);
        }



        private bool DiscountCodeModelExists(int id)
        {
            return _dbContext.DiscountCodes.Any(e => e.Id == id);
        }
    }
}
