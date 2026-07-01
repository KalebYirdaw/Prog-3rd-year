using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using GLMS.Services;
using GLMS.Models;

namespace GLMS.Controllers
{
    public class AccountController : Controller
    {
        private readonly IApiService _apiService;

        public AccountController(IApiService apiService)
        {
            _apiService = apiService;
        }

        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string email, string password, string? returnUrl = null)
        {
            // For demo - hardcoded admin and client
            if (email == "admin@techmove.com" && password == "Admin@123")
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, email),
                    new Claim(ClaimTypes.Email, email),
                    new Claim(ClaimTypes.Role, "Admin"),
                    new Claim("UserRole", "Admin")
                };
                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));

                TempData["Success"] = "Logged in as Admin!";
                return RedirectToAction("AdminDashboard", "Dashboard");
            }

            if (email == "client@techmove.com" && password == "Client@123")
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, email),
                    new Claim(ClaimTypes.Email, email),
                    new Claim(ClaimTypes.Role, "Client"),
                    new Claim("UserRole", "Client")
                };
                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));

                TempData["Success"] = "Logged in as Client!";
                return RedirectToAction("ClientDashboard", "Dashboard");
            }

            // Try to login via API for registered users
            var success = await _apiService.LoginUserAsync(email, password);
            if (success)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, email),
                    new Claim(ClaimTypes.Email, email),
                    new Claim(ClaimTypes.Role, "Client"),
                    new Claim("UserRole", "Client")
                };
                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));

                TempData["Success"] = "Logged in successfully!";
                return RedirectToAction("ClientDashboard", "Dashboard");
            }

            TempData["Error"] = "Invalid email or password";
            return View();
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Register via API
                var success = await _apiService.RegisterUserAsync(model);

                if (success)
                {
                    // Auto login after registration
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, model.Email),
                        new Claim(ClaimTypes.Email, model.Email),
                        new Claim(ClaimTypes.Role, model.Role ?? "Client"),
                        new Claim("UserRole", model.Role ?? "Client")
                    };
                    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));

                    TempData["Success"] = $"Welcome {model.FirstName}! Your account has been created.";

                    if (model.Role == "Admin")
                        return RedirectToAction("AdminDashboard", "Dashboard");
                    return RedirectToAction("ClientDashboard", "Dashboard");
                }

                TempData["Error"] = "Registration failed. Email may already exist.";
            }
            return View(model);
        }

        [Authorize]
        public IActionResult Profile()
        {
            var user = new ApplicationUser
            {
                FirstName = User.Identity?.Name?.Split('@')[0] ?? "Demo",
                LastName = "User",
                Email = User.Identity?.Name ?? "user@example.com",
                UserRole = User.IsInRole("Admin") ? "Admin" : "Client",
                CreatedAt = DateTime.Now.AddDays(-30),
                IsActive = true
            };
            return View(user);
        }

        [Authorize]
        public IActionResult ChangePassword()
        {
            return View(new ChangePasswordViewModel());
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            if (model.NewPassword != model.ConfirmPassword)
            {
                ModelState.AddModelError("", "Passwords do not match.");
                return View(model);
            }

            TempData["Success"] = "Password changed successfully! (Demo)";
            return RedirectToAction(nameof(Profile));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            TempData["Success"] = "Logged out successfully.";
            return RedirectToAction("Login");
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}