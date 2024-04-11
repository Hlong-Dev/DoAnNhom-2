using DoAnNhom_2.Data;
using DoAnNhom_2.Models;
using DoAnNhom_2.Repository;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.IsEssential = true;
});
builder.Services.AddAuthentication().AddGoogle(googleOptions =>
{
    googleOptions.ClientId = configuration["Authentication:Google:ClientId"];
    googleOptions.ClientSecret = configuration["Authentication:Google:ClientSecret"];
});
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();
builder.Services.AddScoped<IDiscountCodeRepository, DiscountCodeRepository>();
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
        .AddDefaultTokenProviders()
        .AddDefaultUI()
        .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddOptions();
var mailsettings = builder.Configuration.GetSection("MailSettings");
builder.Services.Configure<MailSettings>(mailsettings);
builder.Services.AddSingleton<IEmailSender, SendMailService>();
builder.Services.Configure<IdentityOptions>(options =>
{
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(6); 
    options.Lockout.MaxFailedAccessAttempts = 5; 
});

//builder.Services.AddSingleton<INavigationService,NavigationService>();
builder.Services.AddControllersWithViews();

var app = builder.Build();
app.UseSession();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();
app.MapRazorPages();
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
      name: "areas",
      pattern: "{area:exists}/{controller=Product}/{action=Index}/{id?}"
    );
});
app.MapControllerRoute(
    name: "category",
    pattern: "danh-muc/{Slug?}",
    defaults: new { controller = "Category", action = "Index" });
app.MapControllerRoute(
    name: "brand",
    pattern: "thuong-hieu/{Slug?}",
    defaults: new { controller = "Brand", action = "Index" });
//app.MapControllerRoute(
//    name: "Details",
//    pattern: "Details/{slug?}",
//    defaults: new { controller = "Product", action = "Details" });
app.MapControllerRoute(
    name: "product",
    pattern: "san-pham",
    defaults: new { controller = "Product", action = "Index" });
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");


app.Run();
