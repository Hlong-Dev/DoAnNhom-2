using System.Threading.Tasks;
using DoAnNhom_2.Data;
using DoAnNhom_2.Models;
using Microsoft.EntityFrameworkCore;

namespace DoAnNhom_2.Repository
{
    public interface IDiscountCodeRepository
    {
        Task<DiscountCodeModel> GetDiscountCodeByCodeAsync(string code);
        Task<bool> UseDiscountCodeAsync(string code);
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

        public async Task<bool> UseDiscountCodeAsync(string code)
        {
            var discountCode = await _dbContext.DiscountCodes.FirstOrDefaultAsync(dc => dc.Code == code);

            if (discountCode != null && discountCode.Quantity > 0)
            {
                discountCode.Quantity--; // Giảm số lượng còn lại
                await _dbContext.SaveChangesAsync();
                return true; // Trả về true nếu mã giảm giá được sử dụng thành công
            }

            return false; // Trả về false nếu mã không tồn tại hoặc hết hạn
        }

        // Triển khai các phương thức khác nếu cần
    }
}
