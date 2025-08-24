
using System.Diagnostics;
using BCrypt.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using testingSite.Models;
using testingSite.Data;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace testingSite.Controllers;

public class UserController : Controller
{

    private readonly IAppLogger _logger;
    private readonly AppDbContext _context;

    public UserController(IAppLogger logger, AppDbContext context)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<IActionResult> Auth(string returnUrl = null)
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            var role = User.FindFirst(ClaimTypes.Role)?.Value?.ToLower();
            return role switch
            {
                "admin" => RedirectToAction("Dashboard", "Admin"),
                "teacher" => RedirectToAction("Dashboard", "Teacher"),
                "student" => RedirectToAction("Dashboard", "Student"),
                _ => RedirectToAction("Logout")
            };
        }
        ViewBag.ReturnUrl = returnUrl;
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Auth(string username, string password, string returnUrl = null)
    {
        var user = _context.Users.FirstOrDefault(u => u.Username == username);
        if (user != null && BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role.ToLower())
            };
            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                var role = user.Role.ToLower();

                if ((role == "admin" && returnUrl.StartsWith("/admin", StringComparison.OrdinalIgnoreCase)) ||
                    (role == "teacher" && returnUrl.StartsWith("/teacher", StringComparison.OrdinalIgnoreCase)) ||
                    (role == "student" && returnUrl.StartsWith("/student", StringComparison.OrdinalIgnoreCase)))
                {

                    return Redirect(returnUrl);
                    
                }

            }
                _logger.Log(
                user.Id,
                "Вход",
                $"Пользователь с именем {user.Username} совершил вход на сайт."
            );
            return RedirectToAction("Dashboard");
        }
        ViewBag.Error = "Неверный логин или пароль";
        ViewBag.ReturnUrl = returnUrl;
        return View();
    }

    public async Task<IActionResult> Dashboard()
    {
        var role = User.FindFirst(ClaimTypes.Role)?.Value?.ToLower();

        return role switch
        {
            "admin" => RedirectToAction("Dashboard", "Admin"),
            "teacher" => RedirectToAction("Dashboard", "Teacher"),
            "student" => RedirectToAction("Dashboard", "Student"),
            _ => RedirectToAction("Auth")
        };
    }

    public async Task<IActionResult> Logout()
    {
        HttpContext.Session.Clear();
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        _logger.Log(
            int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0"),
            "Выход",
            $"Пользователь с именем {User.FindFirstValue(ClaimTypes.Name)} совершил выход с сайта."
        );
        return RedirectToAction("Auth");
    }

    public async Task<IActionResult> AccessDenied()
    {
        return View();
    }
}
