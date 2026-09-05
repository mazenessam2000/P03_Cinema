using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;

namespace P03_Cinema.Areas.Customer.Controllers
{
    [Area(SD.CUSTOMER_AREA)]
    [Authorize(Roles = SD.CUSTOMER_ROLE)]
    public class ProfileController(UserManager<ApplicationUser> userManager) : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager = userManager;

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
                RedirectToAction(nameof(Login));

            var vm = new UserProfileVM
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Username = user.UserName,
                Email = user.Email
            };

            return View(vm);
        }

        [HttpGet]
        public IActionResult VerifyPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> VerifyPassword(VerifyPasswordVM vm)
        {
            if (!ModelState.IsValid)
                return View(vm);

            var user = await _userManager.GetUserAsync(User);

            if (user == null)
                return RedirectToAction(nameof(Login));

            if (!await _userManager.CheckPasswordAsync(user, vm.Password))
            {
                ModelState.AddModelError("Password", "Incorrect password.");
                return View(vm);
            }

            TempData["VerifiedAt"] = DateTime.UtcNow;

            return RedirectToAction(nameof(EditProfile));
        }

        [HttpGet]
        public IActionResult EditProfile()
        {
            if (!IsRecentlyVerified())
                return RedirectToAction(nameof(VerifyPassword));

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> EditProfile(EditProfileVM vm)
        {
            if (!IsRecentlyVerified())
                return RedirectToAction(nameof(VerifyPassword));

            var user = await _userManager.GetUserAsync(User);

            if (user == null)
                return RedirectToAction(nameof(Login));

            if (!ModelState.IsValid)
                return View(vm);

            // First Name
            if (!string.IsNullOrWhiteSpace(vm.FirstName))
                user.FirstName = vm.FirstName;

            // Last Name
            if (!string.IsNullOrWhiteSpace(vm.LastName))
                user.LastName = vm.LastName;

            // Username
            if (!string.IsNullOrWhiteSpace(vm.Username) && vm.Username != user.UserName)
            {
                var result = await _userManager.SetUserNameAsync(user, vm.Username);
                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                        ModelState.AddModelError(nameof(vm.Username), error.Description);

                    return View(vm);
                }
            }

            // Email
            if (!string.IsNullOrWhiteSpace(vm.Email) && vm.Email != user.Email)
            {
                var result = await _userManager.SetEmailAsync(user, vm.Email);
                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                        ModelState.AddModelError(nameof(vm.Email), error.Description);

                    return View(vm);
                }
            }

            // Password
            if (!string.IsNullOrWhiteSpace(vm.NewPassword))
            {
                if (string.IsNullOrWhiteSpace(vm.CurrentPassword))
                {
                    ModelState.AddModelError(nameof(vm.CurrentPassword), "Current password is required.");
                    return View(vm);
                }

                var passResult = await _userManager.ChangePasswordAsync(
                    user,
                    vm.CurrentPassword,
                    vm.NewPassword
                );

                if (!passResult.Succeeded)
                {
                    foreach (var error in passResult.Errors)
                        ModelState.AddModelError(nameof(vm.CurrentPassword), error.Description);

                    return View(vm);
                }
            }
            // Save Changes
            var updateResult = await _userManager.UpdateAsync(user);

            if (!updateResult.Succeeded)
            {
                foreach (var error in updateResult.Errors)
                    ModelState.AddModelError("", error.Description);

                return View(vm);
            }

            return RedirectToAction(nameof(Index));
        }

        private bool IsRecentlyVerified()
        {
            if (TempData["VerifiedAt"] is DateTime verifiedAt)
            {
                TempData.Keep("VerifiedAt");

                return DateTime.UtcNow - verifiedAt < TimeSpan.FromMinutes(5);
            }

            return false;
        }
    }
}