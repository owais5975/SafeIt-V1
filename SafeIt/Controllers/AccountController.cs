using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using SafeIt.Models;
using System.Security.Claims;

namespace SafeIt.Controllers
{
    public class AccountController : Controller
    {
        //private ApplicationDBContext _context;
        private readonly INotyfService _notyf;

        public AccountController(INotyfService notyf)
        {
            //_context = context;
            _notyf = notyf;
        }
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(UserLoginModel userModel)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View();
                }

                if (userModel != null)
                {
                    if (userModel.Password.Equals("test1234") && userModel.Email.Equals("test@gmail.com"))
                    {
                        var identity = new ClaimsIdentity(new[]
                        {
                            new Claim(ClaimTypes.Name, userModel.Email),
                            new Claim(ClaimTypes.Email, userModel.Email),
                            new Claim(ClaimTypes.Role, "Admin")
                        }, CookieAuthenticationDefaults.AuthenticationScheme);

                        var principal = new ClaimsPrincipal(identity);
                        var login = HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
                        _notyf.Success("Successfuly login.");
                        return RedirectToAction("Home", "Home");
                    }
                }

                _notyf.Error("Incorrect Email or Password.");
                return View();
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public IActionResult Logout()
        {
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }
    }
}
