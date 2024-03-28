using DoAnNhom_2.Data;
using DoAnNhom_2.Models;

using Microsoft.EntityFrameworkCore;

namespace DoAnNhom_2.Repository
{
    public class SeedData
    {
        public static void SeedingData(ApplicationDbContext _context)
        {
            _context.Database.Migrate();
            if (!_context.Products.Any())
            {
                CategoryModel DisPlay = new CategoryModel { Name = "Màn Hình", Slug = "mic", Description = "Apple is Brand in the word", Status = 1 };
                CategoryModel Televison = new CategoryModel { Name = "Televison", Slug = "pc", Description = "Apple is Brand in the word", Status = 1 };

                BrandModel apple = new BrandModel { Name = "Apple", Slug = "mic", Description = "Apple is Brand in the word", Status = 1 };
                BrandModel samsung = new BrandModel { Name = "SamSung", Slug = "pc", Description = "Apple is Brand in the word", Status = 1 };

                _context.Products.AddRange(

                    new ProductModel { Name = "MacBook", Slug = "mic", Description = "mic is best", Image = "mac.jpg", Category = DisPlay, Brand = apple, Price = 1233 },
                    new ProductModel { Name = "pc", Slug = "pc", Description = "mic is best", Image = "1.jpg", Category = Televison, Brand = samsung, Price = 1233 },
                    new ProductModel { Name = "MacBook pro 2019", Slug = "mic", Description = "mic is best", Image = "1.jpg", Category = DisPlay, Brand = apple, Price = 1233 }


                );
                _context.SaveChanges();
            }
        }

    }
}
