using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using testingSite.Models;
using testingSite.Data;
using Microsoft.AspNetCore.Authorization;

namespace testingSite.Controllers;

[Authorize(Roles = "admin")]
public class AdminController : Controller
{
    private readonly AppDbContext _context;
    public AdminController(AppDbContext context)
    {
        _context = context;
    }
    public IActionResult Dashboard()
    {
        return View();
    }
        public IActionResult Register()
    {
        return View();
    }
    // POST: /User/Register
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
            Role = role // "admin", "teacher", "student"
        };
        _context.Users.Add(user);
        _context.SaveChanges();
        return View();
    }
}