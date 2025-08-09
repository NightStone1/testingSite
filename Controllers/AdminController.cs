using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using testingSite.Models;
using testingSite.Data;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace testingSite.Controllers;

[Authorize(Roles = "admin")]
public class AdminController : Controller
{
    private readonly IAppLogger _logger;

    private readonly AppDbContext _context;

    public AdminController(AppDbContext context, IAppLogger logger)
    {
        _context = context;
        _logger = logger;
    }

    public IActionResult Dashboard()
    {
        return View();
    }

    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Register(string username, string password, string role)
    {
        if (_context.Users.Any(u => u.Username == username))
        {
            ModelState.AddModelError("", "Пользователь с таким именем уже существует");
            return View();
        }
        var user = new User
        {
            Username = username,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
            Role = role
        };
        _context.Users.Add(user);
        _context.SaveChanges();        
        _logger.Log(int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0"), "Создание пользователя", $"Создан пользователь: {username} с ролью {role}");
        return View();
    }
}