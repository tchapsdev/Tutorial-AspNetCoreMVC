using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using WebAppMVC.Data;
using WebAppMVC.Models;

namespace WebAppMVC.Controllers
{
    public class UsersController : Controller
    {
        private readonly WebAppMVCContext _context;

        public UsersController(WebAppMVCContext context)
        {
            _context = context;

        }
        public IActionResult Index()
        {
            ViewBag.Title = "Users";

            ViewBag.Country = "CA";

            return View();
        }

        public IActionResult Login(string? ReturnUrl = "")
        {
            ViewBag.Title = "Login";
            ViewBag.ReturnUrl = ReturnUrl;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginModel model, string? ReturnUrl = "")
        {
            ViewBag.Title = "Login";
            if (ModelState.IsValid)
            {
                // Validate the Username and Password
                var user = _context.Customer.Where(c => c.Email == model.UserName)
                    .FirstOrDefault();

                if (user == null)
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    return View(model);
                }
                // Add cookie to users

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.Email),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim("FullName", user.Name),
                    new Claim(ClaimTypes.Role, "Administrator")
                };

                var authProperties = new AuthenticationProperties
                {
                    AllowRefresh = true,
                    // Refreshing the authentication session should be allowed.

                    ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(5),
                    // The time at which the authentication ticket expires. A 
                    // value set here overrides the ExpireTimeSpan option of 
                    // CookieAuthenticationOptions set with AddCookie.

                    IsPersistent = true,
                    // Whether the authentication session is persisted across 
                    // multiple requests. When used with cookies, controls
                    // whether the cookie's lifetime is absolute (matching the
                    // lifetime of the authentication ticket) or session-based.

                    IssuedUtc = DateTime.UtcNow,
                    // The time at which the authentication ticket was issued.

                    //RedirectUri = <string>
                    // The full path or absolute URI to be used as an http 
                    // redirect response value.
                };

                var claimsIdentity = new ClaimsIdentity(
                    claims, CookieAuthenticationDefaults.AuthenticationScheme);

                await HttpContext.SignInAsync(
                       CookieAuthenticationDefaults.AuthenticationScheme,
                       new ClaimsPrincipal(claimsIdentity),
                       authProperties);

                if (!string.IsNullOrEmpty(ReturnUrl))
                {
                    return LocalRedirect(ReturnUrl);
                }
                return RedirectToAction("Index", "Home");

            }
            return View(model);
        }

        public async Task<IActionResult> LogOut()
        {
            await HttpContext.SignOutAsync();

            return RedirectToAction("Index", "Home");
        }


    }
}
