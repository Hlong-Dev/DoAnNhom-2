using DoAnNhom_2.Data;
using DoAnNhom_2.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using static Org.BouncyCastle.Crypto.Engines.SM2Engine;

namespace DoAnNhom_2.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _dataContext;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailSender _emailSender;

        public UserController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IEmailSender emailSender)
        {
            _dataContext = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _emailSender = emailSender;
        }

        [Route("quan-ly-nguoi-dung")]
        public async Task<IActionResult> Index()
        {
            var users = await _dataContext.Users.ToListAsync();
            var roles = await _roleManager.Roles.ToListAsync();

            var viewModel = new UserIndexViewModel
            {
                Users = users,
                Roles = roles
            };

            return View(viewModel);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [Authorize(Roles = SD.Role_Admin)]
        [HttpPost]
        public async Task<IActionResult> Create(ApplicationUser user)
        {
            if (ModelState.IsValid)
            {
                await _userManager.CreateAsync(user);
                return RedirectToAction(nameof(Index));
            }
            return View(user);
        }

        [Authorize(Roles = SD.Role_Admin)]
        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var userRoles = await _userManager.GetRolesAsync(user);
            var roles = await _roleManager.Roles.ToListAsync();

            var viewModel = new UserEditViewModel
            {
                User = user,
                UserRoles = userRoles,
                Roles = roles.Select(r => new SelectListItem
                {
                    Text = r.Name,
                    Value = r.Name,
                    Selected = userRoles.Contains(r.Name)
                }).ToList()
            };

            return View(viewModel);
        }

        [Authorize(Roles = SD.Role_Admin)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, UserEditViewModel model)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                user.UserName = model.User.UserName;
                user.Email = model.User.Email;
                var result = await _userManager.UpdateAsync(user);

                if (result.Succeeded)
                {
                    if (User.IsInRole(SD.Role_Admin))
                    {
                        var userRoles = await _userManager.GetRolesAsync(user);
                        await _userManager.RemoveFromRolesAsync(user, userRoles.ToArray());

                        if (model.SelectedRoles != null && model.SelectedRoles.Any())
                        {
                            await _userManager.AddToRolesAsync(user, model.SelectedRoles);
                        }
                    }
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    ModelState.AddModelError("", "Cập nhật thông tin người dùng không thành công.");
                    return View(model);
                }
            }

            return View(model);
        }
        [Authorize(Roles = SD.Role_Admin)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Lockout(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var result = await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.MaxValue); 

            if (result.Succeeded)
            {
             
                return RedirectToAction(nameof(LockoutSuccess));
            }
            else
            {
                ModelState.AddModelError("", "Khóa tài khoản không thành công.");
                return RedirectToAction(nameof(Index)); 
            }
        }
        [Authorize(Roles = SD.Role_Admin)]
        public IActionResult LockoutSuccess()
        {
            return View();
        }


        [HttpPost]

        public async Task<IActionResult> SendForgotPasswordEmail(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            try
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token)); // Encode token
                var callbackUrl = Url.Action(
                    "ResetPassword",
                    "Account",
                    new { area = "Identity", userId = user.Id, code },
                    protocol: HttpContext.Request.Scheme);

                var emailSubject = "Forgot Password";
                var emailBody = $"Please reset your password by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.";

                await _emailSender.SendEmailAsync(user.Email, emailSubject, emailBody);

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }

    public class UserIndexViewModel
    {
        public List<ApplicationUser> Users { get; set; }
        public List<IdentityRole> Roles { get; set; }
    }

    public class UserEditViewModel
    {
        public ApplicationUser User { get; set; }
        public IList<string> UserRoles { get; set; }
        public List<SelectListItem> Roles { get; set; }
        public IList<string> SelectedRoles { get; set; }
    }
}
