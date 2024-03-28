using DoAnNhom_2.Data;
using DoAnNhom_2.Models;
using DoAnNhom_2.Repository;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DoAnNhom_2.Controllers
{
    public class CheckoutController : Controller
    {
        private readonly ApplicationDbContext _datacontext;
        public CheckoutController(ApplicationDbContext context)
        {
              _datacontext = context;
        }
        public async Task<IActionResult> Checkout()
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            if (userEmail == null)
            {
                return RedirectToAction("Login", "Account");
            }
            else
            {
                var ordercode = Guid.NewGuid().ToString();
                var orderItem = new OrderModel();
                orderItem.Ordercode = ordercode;
                orderItem.UserName = userEmail;
                orderItem.Status = 1;
                orderItem.CreatedDate = DateTime.Now;
                _datacontext.Add(orderItem);
                await _datacontext.SaveChangesAsync(); // Sử dụng SaveChangesAsync và await
                List<CartItemModel> cartitems = HttpContext.Session.GetJson<List<CartItemModel>>("Cart") ?? new List<CartItemModel>();
                foreach (var cart in cartitems)
                {
                    var orderdetails = new OrderDetails();
                    orderdetails.UserName = userEmail;
                    orderdetails.OrderCode = ordercode;
                    orderdetails.ProductId = cart.ProductId;
                    orderdetails.Price = cart.Price;
                    orderdetails.Quantity = cart.Quantity;
                    _datacontext.Add(orderdetails); // Thêm orderdetails vào context
                    await _datacontext.SaveChangesAsync(); // Sử dụng SaveChangesAsync và await
                }
                HttpContext.Session.Remove("Cart");
                TempData["success"] = "Checkout thanh cong";
                return RedirectToAction("ConfirmPaymentClient", "Cart");
            }
            // Không cần return View() ở đây vì trong trường hợp email không hợp lệ, chúng ta đã thực hiện redirect.
        }
    }
}
