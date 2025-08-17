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
        var model = _context.Groups.ToList();
        return View("Register", model);
    }

    [HttpPost]
    public IActionResult Register(string username, string password, string role, int groupSelect)
    {
        var model = _context.Groups.ToList();
        if (_context.Users.Any(u => u.Username == username))
        {
            ViewBag.Error = "Пользователь с таким именем уже существует";
            return PartialView("_RegisterForm", model);
        }
        if (groupSelect != 0 && role != "student")
        {
            ViewBag.Error = "Нельзя создать пользователя с группой и ролью НЕстудент";
            return PartialView("_RegisterForm", model);
        }
        if (groupSelect == 0 && role == "student")
        {
            ViewBag.Error = "Нельзя создать студента без группы";
            return PartialView("_RegisterForm", model);
        }
        ViewBag.Success = "Пользователь создан";
        ModelState.Clear();
        var user = new User
        {
            Username = username,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
            Role = role,
            GroupId = groupSelect
        };
        _context.Users.Add(user);
        _context.SaveChanges();        
        _logger.Log(int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0"), "Создание пользователя", $"Создан пользователь: {username} с ролью {role}");
        return PartialView("_RegisterForm", model);
    }
}