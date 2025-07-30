using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using testingSite.Models;
using testingSite.Data;
using Microsoft.AspNetCore.Authorization;

namespace testingSite.Controllers;

[Authorize(Roles = "admin")]
public class AdminController : Controller
{
    public IActionResult Dashboard()
    {
        return View();
    }
}