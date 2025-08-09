using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using testingSite.Models;
using testingSite.Data;
using Microsoft.AspNetCore.Authorization;

namespace testingSite.Controllers;

[Authorize(Roles = "student")]
public class StudentController : Controller
{
    private readonly IAppLogger _logger;

    private readonly AppDbContext _context;

    public StudentController(AppDbContext context, IAppLogger logger)
    {
        _context = context;
        _logger = logger;
    }

    public IActionResult Dashboard()
    {
        return View();
    }
        public IActionResult Lectures()
    {
        return PartialView("Lectures");
    }

    public IActionResult Tests()
    {
        return PartialView("Tests");
    }
    public IActionResult Progress()
    {
        return PartialView("Progress");
    }
}