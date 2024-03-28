using DoAnNhom_2.Models;
using DoAnNhom_2.Models.ViewModels;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace DoAnNhom_2.Controllers
{
    public class AccountController : Controller
    {
        private UserManager<AppUserModel> _userManager;
        private SignInManager<AppUserModel> _signInManager;
        public AccountController(SignInManager<AppUserModel> signInManager, UserManager<AppUserModel> userManager) 
        { 
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public IActionResult Login(string returnUrl)
        {
            return View(new LoginViewModel { ReturnUrl =returnUrl});
        }
        [HttpPost]
		public async Task<IActionResult> Login(LoginViewModel loginVN)
        {
            if (ModelState.IsValid)
             {
                Microsoft.AspNetCore.Identity.SignInResult result = await _signInManager.PasswordSignInAsync(loginVN.Username, loginVN.Password, false, false);
                if (result.Succeeded)
                {
                    return Redirect(loginVN.ReturnUrl ?? "/");
                }
                ModelState.AddModelError("", "Invalid Username and Password");
            }
            return View(loginVN);
        }
		public IActionResult Create()
		{
			return View();
		}
		[HttpPost]
     
		public async Task<IActionResult> Create(UserModel user)
		{
            if(ModelState.IsValid)
            {
                AppUserModel newUser = new AppUserModel { UserName = user.Username, Email = user.Email };
                IdentityResult result = await _userManager.CreateAsync(newUser,user.Password);
                if (result.Succeeded)
                {
                    TempData["success"] = "Tao User Thanh Cong";
                    return Redirect("/account/login");
                }
                foreach(IdentityError error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
			return View(user);
		}
        public async Task<IActionResult> Logout(string returnUrl="/")
        {
            await _signInManager.SignOutAsync();
            return Redirect(returnUrl);
        }
        [HttpGet("/account/google-login")]
        public IActionResult GoogleLogin(string returnUrl = "/")
        {
            var properties = new AuthenticationProperties
            {
                RedirectUri = Url.Action(nameof(GoogleLoginCallback), new { returnUrl })
            };
            return Challenge(properties, "Google");
        }

        [HttpGet("/account/google-login-callback")]
        public async Task<IActionResult> GoogleLoginCallback(string returnUrl = "/")
        {
            var authenticateResult = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            if (authenticateResult?.Succeeded != true)
            {
                // Handle authentication failure
                return RedirectToAction(nameof(Login));
            }

            // Authentication succeeded, continue with your logic
            return Redirect(returnUrl);
        }

    }
}
