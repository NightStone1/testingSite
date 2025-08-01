using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using testingSite.Models;
using testingSite.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace testingSite.Controllers;

[Authorize(Roles = "teacher")]
public class TeacherController : Controller
{
    private readonly AppDbContext _context;
    public TeacherController(AppDbContext context)
    {
        _context = context;
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
        var categories = _context.TestCategories.Include(tc => tc.Tests).OrderBy(tc => tc.Name).ToList();
        return PartialView("Tests", categories);
    }

    public IActionResult CreateTestCategory()
    {
        return PartialView("CreateTestCategory");
    }
    public IActionResult CreateTest()
    {

        return PartialView("CreateTest");
    }
    public IActionResult Progress()
    {
        return PartialView("Progress");
    }
    public IActionResult LecturesAssignments()
    {
        return PartialView("LecturesAssignments");
    }

    public IActionResult TestsAssignments()
    {
        return PartialView("TestsAssignments");
    }

}