using DoAnNhom_2.Models.ViewModels;
using DoAnNhom_2.Models;
using DoAnNhom_2.Repository;
using Microsoft.AspNetCore.Mvc;
using MoMo;
using Newtonsoft.Json.Linq;
using DoAnNhom_2.Data;
using Microsoft.EntityFrameworkCore;

namespace DoAnNhom_2.Controllers
{
	public class CartController : Controller
	{
		private readonly ApplicationDbContext _dataContext;
		public CartController(ApplicationDbContext _context)
		{
			_dataContext = _context;
		}
		public IActionResult Index()
		{
			List<CartItemModel> cartitems = HttpContext.Session.GetJson<List<CartItemModel>>("Cart") ?? new List<CartItemModel>();
			CartItemViewModel cartVN = new()
			{
				CartItems = cartitems,
				GrandTotal = cartitems.Sum(x => x.Quantity * x.Price),
			};
			return View(cartVN);
		}
		public ActionResult Checkout()
		{
			return View("~/Views/Checkout/Index.cshtml");
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
            int totalAmount = (int)cartitems.Sum(x => x.Quantity * x.Price);

            string endpoint = "https://test-payment.momo.vn/gw_payment/transactionProcessor";
            string partnerCode = "MOMOOJOI20210710";
            string accessKey = "iPXneGmrJH0G8FOP";
            string serectkey = "sFcbSGRSJjwGxwhhcEktCHWYUuTuPNDB";
            string orderInfo = "Thanh Toán Đơn Hàng";
            string returnUrl = "http://localhost:5108/Cart/ConfirmPaymentClient";
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

        public ActionResult ConfirmPaymentClient(Result result)
        {
            //lấy kết quả Momo trả về và hiển thị thông báo cho người dùng (có thể lấy dữ liệu ở đây cập nhật xuống db)
            string rMessage = result.message;
            string rOrderId = result.orderId;
            string rErrorCode = result.errorCode; // = 0: thanh toán thành công
            return View();
        }

        [HttpPost]
        public IActionResult SavePayment()
        {
            //cập nhật dữ liệu vào db
            // Sau khi lưu trữ thành công, trả về một StatusCode
            return StatusCode(200); // 200 là mã trạng thái thành công
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


    }
}
