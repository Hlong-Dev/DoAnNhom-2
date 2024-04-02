using DoAnNhom_2.Data;
using DoAnNhom_2.Models;
using DoAnNhom_2.Repository;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DoAnNhom_2.Controllers
{
    public class CheckoutController : Controller
    {
        private readonly ApplicationDbContext _datacontext;
        private readonly UserManager<IdentityUser> _userManager;

        public CheckoutController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _datacontext = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Checkout(string fullName, string phoneNumber, string address, string note)
        {
            // Lấy thông tin người dùng từ Identity
            IdentityUser user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                // Nếu người dùng không đăng nhập, chuyển hướng đến trang đăng nhập
                return RedirectToAction("Login", "Account", new { area = "Identity" });
            }
            else
            {

                // Tạo mã đơn hàng
                var orderCode = Guid.NewGuid().ToString();
                var orderItem = new OrderModel
                {
                    Ordercode = orderCode,
                    UserName = user.UserName,
       
                    Status = 1, // Giả sử status 1 là đơn hàng mới
                    CreatedDate = DateTime.Now
                };

                // Thêm đơn hàng vào database
                _datacontext.Add(orderItem);
                await _datacontext.SaveChangesAsync();

                // Lấy thông tin giỏ hàng từ Session
                List<CartItemModel> cartItems = HttpContext.Session.GetJson<List<CartItemModel>>("Cart") ?? new List<CartItemModel>();

                // Tính tổng số lượng và tổng giá tiền của tất cả các mục OrderDetails
                int totalQuantity = cartItems.Sum(od => od.Quantity);
                decimal totalPrice = cartItems.Sum(od => od.Price * od.Quantity);

                // Tạo một mục OrderDetails mới đại diện cho tổng của cả hai đơn hàng
                var totalOrderDetails = new OrderDetails
                {
                    OrderCode = orderCode,
                    ProductId = cartItems.Count, // Sử dụng một ID duy nhất để đại diện cho tổng của cả hai đơn hàng
                    Price = totalPrice,
                    Quantity = totalQuantity,
                    PhoneNumber = phoneNumber, // Sử dụng số điện thoại từ form
                    UserName = user.UserName,
                    Address = address, // Sử dụng địa chỉ từ form
                    FullName = fullName, // Sử dụng họ tên từ form
                                         // Sử dụng ghi chú từ form
                };

                // Thêm mục OrderDetails đại diện cho tổng vào database
                _datacontext.Add(totalOrderDetails);

                // Xóa thông tin giỏ hàng từ Session
                HttpContext.Session.Remove("Cart");

                // Lưu các thay đổi vào database
                await _datacontext.SaveChangesAsync();

                // Chuyển hướng đến trang xác nhận thanh toán
                return RedirectToAction("ConfirmPaymentClient", "Cart");
            }
        }
    }
}
