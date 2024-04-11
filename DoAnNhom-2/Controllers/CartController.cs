using DoAnNhom_2.Models.ViewModels;
using DoAnNhom_2.Models;
using DoAnNhom_2.Repository;
using Microsoft.AspNetCore.Mvc;
using MoMo;
using Newtonsoft.Json.Linq;
using DoAnNhom_2.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.Scripting;
using System.Net;

namespace DoAnNhom_2.Controllers
{
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _dataContext;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IDiscountCodeRepository _discountCodeRepository;
        public CartController(ApplicationDbContext _context, UserManager<ApplicationUser> userManage, IDiscountCodeRepository discountCodeRepository)
        {
            _dataContext = _context;
            _userManager = userManage;
            _discountCodeRepository = discountCodeRepository;
        }
        [Route("gio-hang")]
        public IActionResult Index()
        {
            List<CartItemModel> cartitems = HttpContext.Session.GetJson<List<CartItemModel>>("Cart") ?? new List<CartItemModel>();

            // Tính tổng số tiền trước giảm giá
            decimal totalBeforeDiscount = cartitems.Sum(x => x.Quantity * x.Price);

            // Tính tổng số tiền đã giảm giá
            decimal totalDiscount = cartitems.Sum(x => x.Discount);

            // Tính tổng số tiền sau khi đã giảm giá
            decimal grandTotal = totalBeforeDiscount - totalDiscount;

            // Tính tổng giá cũ sau khi đã giảm giá

            UserIndexViewModel cartVN = new()
            {
                CartItems = cartitems,
                GrandTotal = grandTotal,
                Total = totalBeforeDiscount,
                Discound = totalDiscount,

            };

            ViewBag.CartItemCount = GetCartItemCount(); // Truyền số lượng sản phẩm qua ViewBag
            return View(cartVN);
        }

        // Phương thức để lấy số lượng sản phẩm trong giỏ hàng
        private int GetCartItemCount()
        {
            List<CartItemModel> cart = HttpContext.Session.GetJson<List<CartItemModel>>("Cart") ?? new List<CartItemModel>();
            return cart.Sum(item => item.Quantity);
        }

        [HttpPost]
        public async Task<IActionResult> Add(int Id)
        {
            ProductModel product = await _dataContext.Products.FindAsync(Id);
            List<CartItemModel> cart = HttpContext.Session.GetJson<List<CartItemModel>>("Cart") ?? new List<CartItemModel>();
            CartItemModel cartItem = cart.FirstOrDefault(c => c.ProductId == Id);

            if (cartItem != null)
            {
                cartItem.Quantity += 1;
            }
            else
            {
                cart.Add(new CartItemModel(product));
            }

            HttpContext.Session.SetJson("Cart", cart);

            return Json(new { success = true, message = "Đã Thêm Sản Phẩm Vào Giỏ Hàng Của Bạn Thành Công Rồi Nè" });
        }

        public async Task<IActionResult> Decrease(int Id)
        {
            List<CartItemModel> cart = HttpContext.Session.GetJson<List<CartItemModel>>("Cart");
            CartItemModel cartItem = cart.FirstOrDefault(c => c.ProductId == Id);

            if (cartItem != null && cartItem.Quantity > 1)
            {
                cartItem.Quantity--;
            }
            else
            {
                cart.RemoveAll(p => p.ProductId == Id);
            }

            UpdateSessionCart(cart);
            TempData["success"] = "Bớt Sản Phẩm Trong Giỏ Hàng Thành Công";
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Increase(int Id)
        {
            List<CartItemModel> cart = HttpContext.Session.GetJson<List<CartItemModel>>("Cart");
            CartItemModel cartItem = cart.FirstOrDefault(c => c.ProductId == Id);

            if (cartItem != null)
            {
                cartItem.Quantity++;
            }

            UpdateSessionCart(cart);
            TempData["success"] = "Thêm Sản Phẩm Vào Giỏ Hàng Thành Công";
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Remove(int Id)
        {
            List<CartItemModel> cart = HttpContext.Session.GetJson<List<CartItemModel>>("Cart");
            cart.RemoveAll(p => p.ProductId == Id);
            UpdateSessionCart(cart);
            TempData["success"] = "Xóa Sản Phẩm Trong Giỏ Hàng Thành Công";
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Clear()
        {
            HttpContext.Session.Remove("Cart");
            TempData["success"] = "Xóa Tất Cả Sản Phẩm Trong Giỏ Hàng Thành Công";
            return RedirectToAction("Index");
        }

        private void UpdateSessionCart(List<CartItemModel> cart)
        {
            if (cart.Count == 0)
            {
                HttpContext.Session.Remove("Cart");
            }
            else
            {
                HttpContext.Session.SetJson("Cart", cart);
            }
        }

        public IActionResult Payment()
        {
            List<CartItemModel> cartitems = HttpContext.Session.GetJson<List<CartItemModel>>("Cart") ?? new List<CartItemModel>();
            int totalDiscount = (int)cartitems.Sum(x => x.Discount);
            int totalAmount = (int)cartitems.Sum(x => x.Quantity * x.Price) - totalDiscount; // Tổng tiền sau khi áp dụng giảm giá
            TempData["TotalAmount"] = totalAmount;

            string endpoint = "https://test-payment.momo.vn/gw_payment/transactionProcessor";
            string partnerCode = "MOMOOJOI20210710";
            string accessKey = "iPXneGmrJH0G8FOP";
            string serectkey = "sFcbSGRSJjwGxwhhcEktCHWYUuTuPNDB";
            string orderInfo = "Thanh Toán Đơn Hàng";
            string returnUrl = "https://localhost:7261/Cart/ConfirmPaymentClient";
            string notifyurl = "https://4c8d-2001-ee0-5045-50-58c1-b2ec-3123-740d.ap.ngrok.io/Home/SavePayment";

            string amount = totalAmount + "";
            string orderid = DateTime.Now.Ticks.ToString();
            string requestId = DateTime.Now.Ticks.ToString();
            string extraData = "";


            string rawHash = "partnerCode=" +
                partnerCode + "&accessKey=" +
                accessKey + "&requestId=" +
                requestId + "&amount=" +
                amount + "&orderId=" +
                orderid + "&orderInfo=" +
                orderInfo + "&returnUrl=" +
                returnUrl + "&notifyUrl=" +
                notifyurl + "&extraData=" +
                extraData;

            MoMoSecurity crypto = new MoMoSecurity();

            string signature = crypto.signSHA256(rawHash, serectkey);

            JObject message = new JObject
            {
                { "partnerCode", partnerCode },
                { "accessKey", accessKey },
                { "requestId", requestId },
                { "amount", amount },
                { "orderId", orderid },
                { "orderInfo", orderInfo },
                { "returnUrl", returnUrl },
                { "notifyUrl", notifyurl },
                { "extraData", extraData },
                { "requestType", "captureMoMoWallet" },
                { "signature", signature }
            };

            string responseFromMomo = PaymentRequest.sendPaymentRequest(endpoint, message.ToString());
            JObject jmessage = JObject.Parse(responseFromMomo);
            var payUrlToken = jmessage.GetValue("payUrl");

            if (payUrlToken != null)
            {
                string payUrl = payUrlToken.ToString();
                return Redirect(payUrl);
            }
            else
            {

                return RedirectToAction("Error");
            }
        }

        public IActionResult Error()
        {
            // Có thể thêm logic xử lý lỗi ở đây nếu cần thiết
            return View();
        }

        public async Task<IActionResult> ConfirmPaymentClient(Result result,string fullName, string phoneNumber, string address)
        {
            ApplicationUser user = await _userManager.GetUserAsync(User);
            var orderCode = Guid.NewGuid().ToString();
            var orderItem = new OrderModel
            {
                Ordercode = orderCode,
                UserName = user.UserName,

                Status = 1, // Giả sử status 1 là đơn hàng mới
                CreatedDate = DateTime.Now
            };

            // Thêm đơn hàng vào database
            _dataContext.Add(orderItem);
            await _dataContext.SaveChangesAsync();
            // Lấy thông tin giỏ hàng từ Session
            List<CartItemModel> cartItems = HttpContext.Session.GetJson<List<CartItemModel>>("Cart") ?? new List<CartItemModel>();

            // Tính tổng số lượng và tổng giá tiền của tất cả các mục OrderDetails
            int totalQuantity = cartItems.Sum(od => od.Quantity);
            decimal totalPrice = cartItems.Sum(od => od.Price * od.Quantity);
          

          
            string rMessage = result.message;
            string rOrderId = result.orderId;
            string rErrorCode = result.errorCode;

            // Tạo một đối tượng MoMoPayment để lưu vào cơ sở dữ liệu
            MoMoPayment payment = new MoMoPayment
            {
                OrderId = rOrderId,
                Message = rMessage,
                ErrorCode = rErrorCode,
                Price = totalPrice,
                Quantity = totalQuantity,
                PhoneNumber = user.PhoneNumber, // Sử dụng số điện thoại từ form
                OrderCode = orderCode,
                ProductId= cartItems.Count,
                Address = address, // Sử dụng địa chỉ từ form
                UserName = user.UserName,
                // Các trường dữ liệu khác tùy thuộc vào yêu cầu của ứng dụng của bạn
            };

            // Thêm payment vào cơ sở dữ liệu
            _dataContext.MoMoPayments.Add(payment);
            HttpContext.Session.Remove("Cart");
            await _dataContext.SaveChangesAsync();

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SavePayment(MoMoPayment payment)
        {
            _dataContext.MoMoPayments.Add(payment);
            await _dataContext.SaveChangesAsync();

            
            return StatusCode(200); 
        }

        public IActionResult Details(int id)
        {
            var product = _dataContext.Products.Include(p => p.Category).Include(p => p.Brand).FirstOrDefault(p => p.Id == id && p.IsDeleted == false);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }
        [HttpPost]
        public async Task<IActionResult> ApplyDiscount(string discountCode)
        {
            List<CartItemModel> cartItems = HttpContext.Session.GetJson<List<CartItemModel>>("Cart") ?? new List<CartItemModel>();

            var discountCodeEntity = await _discountCodeRepository.GetDiscountCodeByCodeAsync(discountCode);
            if (discountCodeEntity != null)
            {
                decimal discountRate = discountCodeEntity.Discount;
                string successMessage = $"Đã áp dụng mã giảm giá {discountCode} thành công.";

                foreach (var item in cartItems)
                {
                    decimal discountAmount = item.Price * item.Quantity * discountRate;
                    item.Discount = discountAmount;
                }

                HttpContext.Session.SetJson("Cart", cartItems);
                TempData["success"] = successMessage;
            }
            else
            {
                TempData["error"] = "Mã giảm giá không hợp lệ.";
            }

            return RedirectToAction("Index");
        }


    }
}

