using DoAnNhom_2.Data;
using DoAnNhom_2.Models;
using Microsoft.EntityFrameworkCore;
using System;

namespace DoAnNhom_2.Repository
{
    public interface IDiscountCodeRepository
    {
        Task<DiscountCodeModel> GetDiscountCodeByCodeAsync(string code);
        // Các phương thức khác nếu cần
    }

    public class DiscountCodeRepository : IDiscountCodeRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public DiscountCodeRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<DiscountCodeModel> GetDiscountCodeByCodeAsync(string code)
        {
            return await _dbContext.DiscountCodes.FirstOrDefaultAsync(dc => dc.Code == code);
        }

        // Triển khai các phương thức khác nếu cần
    }
}
