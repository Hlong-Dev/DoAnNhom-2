using DoAnNhom_2.Data;
using DoAnNhom_2.Models;
using DoAnNhom_2.Repository;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DoAnNhom_2.Controllers
{
    public class CheckoutController : Controller
    {
        private readonly ApplicationDbContext _datacontext;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailSender _emailSender;

        public CheckoutController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IEmailSender emailSender)
        {
            _datacontext = context;
            _userManager = userManager;
            _emailSender = emailSender;
        }

        public async Task<IActionResult> Checkout(string fullName, string phoneNumber, string address, string note)
        {
            // Lấy thông tin người dùng từ Identity
            ApplicationUser user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
         
                return RedirectToAction("Login", "Account", new { area = "Identity" });
            }
            else
            {
                var existingOrder = await _datacontext.Orders.FirstOrDefaultAsync(o => o.UserName == user.UserName && o.Status == 1);
                if (existingOrder != null)
                {
                    _datacontext.Orders.Remove(existingOrder);
                    await _datacontext.SaveChangesAsync();
                }

            
                var orderCode = Guid.NewGuid().ToString();
                var orderItem = new OrderModel
                {
                    FullName = fullName,
                    Ordercode = orderCode,
                    UserName = user.UserName,
                    Status = 1,
                    CreatedDate = DateTime.Now
                };

                _datacontext.Add(orderItem);
                await _datacontext.SaveChangesAsync();

                List<CartItemModel> cartItems = HttpContext.Session.GetJson<List<CartItemModel>>("Cart") ?? new List<CartItemModel>();
                int totalQuantity = cartItems.Sum(od => od.Quantity);
                decimal totalPrice = cartItems.Sum(od => od.Price * od.Quantity);

                var totalOrderDetails = new OrderDetails
                {
                    OrderCode = orderCode,
                    ProductId = cartItems.Count,
                    Price = totalPrice,
                    Quantity = totalQuantity,
                    PhoneNumber = phoneNumber,
                    UserName = user.UserName,
                    Address = address,
                    FullName = fullName,
                    CreatedDate = DateTime.Now
                };

                _datacontext.Add(totalOrderDetails);

              
                HttpContext.Session.Remove("Cart");

                await _datacontext.SaveChangesAsync();
                string subject = "Đặt Hàng Thành Công";
                string message = $"Đơn hàng của bạn đã được nhận và đang được xử lý.\n\nThông tin đơn hàng:\n- Mã đơn hàng: {orderCode}\n- Số tiền đơn hàng: {totalPrice.ToString("C")}\n- Họ và tên: {fullName}\n- Số điện thoại: {phoneNumber}\n- Địa chỉ: {address}\n- Ghi chú: {note}";
                await _emailSender.SendEmailAsync(user.Email, subject, message);

                // Chuyển hướng đến trang xác nhận thanh toán
                return RedirectToAction("ConfirmPaymentClient", "Cart");
            }
        }

    }
}

    