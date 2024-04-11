using DoAnNhom_2.Data;
using DoAnNhom_2.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DoAnNhom_2.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _dataContext;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public UserController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _dataContext = context;
            _roleManager = roleManager;
            _userManager = userManager;
        }

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
                // Cập nhật thông tin người dùng
                user.UserName = model.User.UserName;
                user.Email = model.User.Email;
                var result = await _userManager.UpdateAsync(user);

                if (result.Succeeded)
                {
                    // Cập nhật vai trò
                    var userRoles = await _userManager.GetRolesAsync(user);
                    await _userManager.RemoveFromRolesAsync(user, userRoles.ToArray());

                    if (model.SelectedRoles != null && model.SelectedRoles.Any())
                    {
                        await _userManager.AddToRolesAsync(user, model.SelectedRoles);
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

        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            await _userManager.DeleteAsync(user);

            return RedirectToAction(nameof(Index));
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
