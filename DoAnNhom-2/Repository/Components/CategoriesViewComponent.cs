using DoAnNhom_2.Data;
using DoAnNhom_2.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DoAnNhom_2.Repository.Components
{
    public class CategoriesViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _dataContext;
        public CategoriesViewComponent(ApplicationDbContext context)
        {
            _dataContext = context;
        }
      
        public async Task<IViewComponentResult> InvokeAsync()
        {
            try
            {
				var result = await _dataContext.Categories.ToListAsync();
				return View(result);
			}
            catch (Exception ex)
            {
                View(ex); return View();
            }
            
            //return View();
        }

    }
}
