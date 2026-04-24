using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace P03_Cinema.Areas.Identity.Controllers
{
    [Area(SD.IDENTITY_AREA)]
    public class AccountController(UserManager<ApplicationUser> userManager
        , SignInManager<ApplicationUser> signInManager
        , IEmailSender emailSender
        , IRepository<ApplicationUserOTP> applicationUserOTPRepo
        , IUnitOfWork unitOfWork) : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly SignInManager<ApplicationUser> _signInManager = signInManager;
        private readonly IEmailSender _emailSender = emailSender;
        private readonly IRepository<ApplicationUserOTP> _applicationUserOTPRepo = applicationUserOTPRepo;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        [HttpGet]
        public IActionResult Register() => View();

        [HttpPost]
        public async Task<IActionResult> Register(RegisterVM vm)
        {
            if (!ModelState.IsValid)
                return View(vm);

            var user = new ApplicationUser()
            {
                FirstName = vm.FirstName,
                LastName = vm.LastName,
                UserName = vm.Username,
                Email = vm.Email
            };

            var result = await _userManager.CreateAsync(user, vm.Password);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    if (error.Code.Contains("Password"))
                        ModelState.AddModelError(nameof(vm.Password),
                            "Password must be at least 8 characters long and include uppercase, lowercase, a number, and a special character.");
                    else
                        ModelState.AddModelError(string.Empty, error.Description);
                }
                return View(vm);
            }

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var link = Url.Action("Confirm", "Account", new { area = "Identity", token, userId = user.Id }, Request.Scheme);
            await _emailSender.SendEmailAsync(user.Email!, "Account confirmation",
                $"<h1>Please confirm your email address by clicking <a href='{link}'>here</a></h1>");

            TempData["AuthMessage"] = System.Text.Json.JsonSerializer.Serialize(new AuthMessageVM
            {
                Title = "Verify your account",
                Message = $"We sent a verification link to {user.Email}.",
                Email = user.Email,
                IconPath = "/assets/admin/img/icons/spot-illustrations/16.png"
            });

            return RedirectToAction(nameof(AuthMessage));
        }

        public async Task<IActionResult> Confirm(string token, string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user is null) return NotFound();

            var result = await _userManager.ConfirmEmailAsync(user, token);

            if (!result.Succeeded)
            {
                TempData["Error"] = string.Join(", ", result.Errors.Select(e => e.Description));
                return RedirectToAction(nameof(Login));
            }

            TempData["AuthMessage"] = System.Text.Json.JsonSerializer.Serialize(new AuthMessageVM
            {
                Title = "Your account is verified",
                Message = "You can now log in.",
                IconPath = "/assets/admin/img/icons/spot-illustrations/success.png"
            });

            return RedirectToAction(nameof(AuthMessage));
        }

        [HttpGet]
        public IActionResult AuthMessage()
        {
            var json = TempData["AuthMessage"] as string;
            if (json is null) return RedirectToAction(nameof(Login));

            TempData.Keep("PendingOtpUserId");

            var vm = System.Text.Json.JsonSerializer.Deserialize<AuthMessageVM>(json);
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResendLink(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return RedirectToAction(nameof(Login));

            var user = await _userManager.FindByEmailAsync(email);

            if (user is not null && !await _userManager.IsEmailConfirmedAsync(user))
            {
                var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

                var link = Url.Action(
                    "Confirm",
                    "Account",
                    new { area = "Identity", token, userId = user.Id },
                    Request.Scheme
                );

                await _emailSender.SendEmailAsync(
                    user.Email!,
                    "Account confirmation",
                    $"<h1>Please confirm your email by clicking <a href='{link}'>here</a></h1>"
                );
            }

            TempData["AuthMessage"] = System.Text.Json.JsonSerializer.Serialize(new AuthMessageVM
            {
                Title = "Verify your account",
                Message = $"We sent a verification link to {email}.",
                Email = email,
                IconPath = "/assets/admin/img/icons/spot-illustrations/16.png"
            });

            return RedirectToAction(nameof(AuthMessage));
        }

        [HttpGet]
        public IActionResult Login() => View();

        [HttpPost]
        public async Task<IActionResult> Login(LoginVM vm)
        {
            if (!ModelState.IsValid)
                return View(vm);

            var user = await _userManager.FindByEmailAsync(vm.UsernameOrEmail) ??
                       await _userManager.FindByNameAsync(vm.UsernameOrEmail);

            if (user is null)
            {
                ModelState.AddModelError(string.Empty, "Invalid credentials. Please try again.");
                return View(vm);
            }

            if (!await _userManager.IsEmailConfirmedAsync(user))
            {
                ModelState.AddModelError(string.Empty, "You need to confirm your account before logging in. Please check your email.");
                return View(vm);
            }

            var result = await _signInManager.PasswordSignInAsync(user, vm.Password, vm.RememberMe, lockoutOnFailure: true);

            if (result.IsLockedOut)
            {
                ModelState.AddModelError(string.Empty, "Too many attempts. Please try again later.");
                return View(vm);
            }

            if (!result.Succeeded)
            {
                ModelState.AddModelError(string.Empty, "Invalid credentials. Please try again.");
                return View(vm);
            }

            TempData["Success"] = $"Welcome back {user.FirstName} {user.LastName}";
            return RedirectToAction("Index", "Home", new { area = "Customer" });
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return View();
        }

        [HttpGet]
        public IActionResult ForgotPassword() => View();

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordVM vm)
        {
            if (!ModelState.IsValid)
                return View(vm);

            var user = await _userManager.FindByEmailAsync(vm.Email);

            if (user != null)
            {
                var otp = new Random().Next(100000, 999999);

                string subject = "Reset your password";
                string message = $@"
                    <div style='font-family: sans-serif; max-width: 500px; margin: 0 auto; border: 1px solid #e0e0e0; padding: 20px; border-radius: 8px;'>
                        <h2 style='color: #333;'>Password Reset Request</h2>
                        <p style='color: #666; line-height: 1.5;'>
                            We received a request to reset your password. Use the verification code below to proceed:
                        </p>
                        <div style='background-color: #f4f7fa; padding: 20px; text-align: center; border-radius: 6px; margin: 20px 0;'>
                            <span style='font-size: 32px; font-weight: bold; letter-spacing: 5px; color: #2c7be5;'>{otp}</span>
                        </div>
                        <p style='color: #999; font-size: 12px;'>
                            This code will expire in 10 minutes. If you did not request this, please ignore this email.
                        </p>
                        <hr style='border: none; border-top: 1px solid #eee; margin: 20px 0;'>
                        <p style='color: #bbb; font-size: 11px; text-align: center;'>
                            &copy; {DateTime.Now.Year} CinemaVerse. Secure Password Recovery.
                        </p>
                    </div>";

                await _emailSender.SendEmailAsync(user.Email!, subject, message);

                await _applicationUserOTPRepo.AddAsync(new ApplicationUserOTP
                {
                    OTP = otp.ToString(),
                    ApplicationUserId = user.Id
                });

                await _unitOfWork.SaveChangesAsync();

                TempData["PendingOtpUserId"] = user.Id;
            }

            TempData["AuthMessage"] = System.Text.Json.JsonSerializer.Serialize(new AuthMessageVM
            {
                Title = "Check your email",
                Message = $"If {vm.Email} is registered, we sent a 6-digit code. Enter it on the next screen.",
                IconPath = "/assets/admin/img/icons/spot-illustrations/16.png",
                RedirectAction = "ValidateOTP"
            });

            return RedirectToAction(nameof(AuthMessage));
        }

        [HttpGet]
        public IActionResult ValidateOTP()
        {
            var userId = TempData["PendingOtpUserId"]?.ToString();

            if (string.IsNullOrEmpty(userId))
                return RedirectToAction(nameof(ForgotPassword));

            return View(new ValidateOTPVM { UserId = userId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ValidateOTP(ValidateOTPVM vm)
        {
            if (!ModelState.IsValid)
                return View(vm);

            var otpRecord = await _applicationUserOTPRepo
                .Get()
                .Where(o => o.ApplicationUserId == vm.UserId && o.OTP == vm.OTP)
                .OrderByDescending(o => o.CreatedAt)
                .FirstOrDefaultAsync();

            if (otpRecord == null || otpRecord.CreatedAt < DateTime.UtcNow.AddMinutes(-10))
            {
                ModelState.AddModelError(string.Empty, "Invalid or expired code.");
                return View(vm);
            }

            var allOtps = _applicationUserOTPRepo.Get()
                .Where(o => o.ApplicationUserId == vm.UserId);
            foreach (var o in allOtps)
                _applicationUserOTPRepo.Remove(o);

            await _unitOfWork.SaveChangesAsync();

            TempData["OtpVerifiedUserId"] = vm.UserId;

            return RedirectToAction(nameof(ResetPassword));
        }

        [HttpGet]
        public IActionResult ResetPassword()
        {
            var userId = TempData["OtpVerifiedUserId"]?.ToString();

            if (string.IsNullOrEmpty(userId))
                return RedirectToAction(nameof(ForgotPassword));

            TempData.Keep("OtpVerifiedUserId");

            return View(new ResetPasswordVM { UserId = userId });
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordVM vm)
        {
            var verifiedUserId = TempData["OtpVerifiedUserId"]?.ToString();

            if (string.IsNullOrEmpty(verifiedUserId) || verifiedUserId != vm.UserId)
            {
                return RedirectToAction(nameof(ForgotPassword));
            }

            if (!ModelState.IsValid)
            {
                TempData["OtpVerifiedUserId"] = verifiedUserId;
                return View(vm);
            }

            var user = await _userManager.FindByIdAsync(vm.UserId);
            if (user == null)
                return RedirectToAction(nameof(ForgotPassword));

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, token, vm.Password);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                    ModelState.AddModelError(string.Empty, error.Description);

                TempData["OtpVerifiedUserId"] = verifiedUserId;
                return View(vm);
            }

            TempData["AuthMessage"] = System.Text.Json.JsonSerializer.Serialize(new AuthMessageVM
            {
                Title = "Password has been changed",
                Message = "You can now log in.",
                IconPath = "/assets/admin/img/icons/spot-illustrations/success.png"
            });

            return RedirectToAction(nameof(AuthMessage));
        }
    }
}