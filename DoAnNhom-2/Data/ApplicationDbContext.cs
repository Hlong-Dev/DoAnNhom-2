using DoAnNhom_2.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DoAnNhom_2.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<BrandModel> Brands { get; set; }
        public DbSet<ProductModel> Products { get; set; }
        public DbSet<CategoryModel> Categories { get; set; }
        public DbSet<OrderModel> Orders { get; set; }
        public DbSet<OrderDetails> OrderDetails { get; set; }
        public DbSet<MoMoPayment> MoMoPayments { get; set; }
        public DbSet<Result> Results { get; set; }
        public DbSet<DiscountCodeModel> DiscountCodes { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
         
        }
    }
}