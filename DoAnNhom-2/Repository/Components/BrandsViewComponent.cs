using DoAnNhom_2.Data;
using DoAnNhom_2.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DoAnNhom_2.Repository.Components
{
    public class BrandsViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _dataContext;
        public BrandsViewComponent(ApplicationDbContext context)
        {
            _dataContext = context;
        }

        public async Task<IViewComponentResult> InvokeAsync() => View(await _dataContext.Brands.ToListAsync());

    }
}
