using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using testingSite.Models;
using testingSite.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace testingSite.Controllers;

[Authorize(Roles = "teacher")]
public class TeacherController : Controller
{
    private readonly IAppLogger _logger;

    private readonly AppDbContext _context;

    public TeacherController(AppDbContext context, IAppLogger logger)
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

    public IActionResult Questions()
    {
        return PartialView("Questions");
    }

    public IActionResult Tests()
    {
        var testCategories = _context.TestCategories.Include(tc => tc.Tests).Include(tc => tc.Discipline).OrderBy(tc => tc.Name).ToList();
        return PartialView("Tests", testCategories);
    }

    public IActionResult EditTestCategory(int id)
    {
        var category = _context.TestCategories.FirstOrDefault(tc => tc.Id == id);
        if (category == null)
            return NotFound();

        return PartialView("EditTestCategory", category);
    }

    public IActionResult EditTest(int id)
    {
        var test = _context.Tests.Include(t => t.Questions).ThenInclude(q => q.Answers).FirstOrDefault(c => c.Id == id);
        if (test == null)
            return NotFound();

        return PartialView("EditTest", test);
    }

    public IActionResult EditQuestion(int Id)
    {
        var question = _context.Questions.Include(q => q.Answers).FirstOrDefault(c => c.Id == Id);
        if (question == null)
            return NotFound();
        return PartialView("EditQuestion", question);
    }
    [HttpPost]
    public IActionResult EditQuestion(int Id, string questionText)
    {
        var question = _context.Questions.Include(q => q.Answers).Include(q => q.Test).FirstOrDefault(c => c.Id == Id);
        if (question == null)
            return NotFound();
        var oldQuestionText = question.QuestionText;
        if (oldQuestionText != questionText)
        {
            question.QuestionText = questionText;
            _context.SaveChanges();
            _logger.Log(
                int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0"),
                "Изменение вопроса",
                $"Изменен текст вопроса с {oldQuestionText} на {questionText} в тесте {question.Test.Name}"
        );
        }
        return Ok();
    }

    [HttpPost]
    public IActionResult EditTestCategory(string name, int id)
    {
        var category = _context.TestCategories.Include(tc => tc.Discipline).FirstOrDefault(tc => tc.Id == id);
        if (category == null)
            return NotFound();

        var oldName = category.Name;
        if (oldName != name)
        {
            category.Name = name;
            _context.SaveChanges();
            _logger.Log(
                int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0"),
                "Изменение категории",
                $"Изменено имя категории с {oldName} на {name} в дисциплине {category.Discipline.Name}"
        );
        }
        return Ok();
    }

    [HttpPost]
    public IActionResult EditTest(string name, string description, int id)
    {
        var test = _context.Tests.Include(t => t.TestCategory).FirstOrDefault(t => t.Id == id);
        if (test == null)
            return NotFound();

        var oldName = test.Name;
        var oldDescription = test.Description;
        if (oldName != name)
        {
            test.Name = name;
            _context.SaveChanges();
            _logger.Log(
            int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0"),
            "Изменение Теста",
            $"Изменено имя теста с '{oldName}' на '{name}' в категории '{test.TestCategory}'"
        );
        }
        if (oldDescription != description)
        {
            test.Description = description;
            _context.SaveChanges();
            _logger.Log(
            int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0"),
            "Изменение Теста",
            $"Изменено описание теста с '{oldDescription}' на '{description}' в категории '{test.TestCategory}'");
        }        
        return Ok();
    }


    public IActionResult CreateTestCategory()
    {
        var disciplines = _context.Disciplines.OrderBy(d => d.Name).ToList();
        return PartialView("CreateTestCategory", disciplines);
    }

    [HttpPost]
    public IActionResult CreateTestCategory(string name, string disciplineId, string disciplineName)
    {
        if (!string.IsNullOrWhiteSpace(disciplineId) && int.TryParse(disciplineId, out int parsedId))
        {
            _context.TestCategories.Add(new TestCategory
            {
                Name = name,
                DisciplineId = parsedId
            });
            _context.SaveChanges();
            _logger.Log(
                int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0"),
                "Создание категории",
                $"Создана категория: {name} в дисциплине: {disciplineName}"
            );
            return Ok();
        }
        var disciplines = _context.Disciplines.OrderBy(d => d.Name).ToList();
        return PartialView("CreateTestCategory", disciplines);
    }
    public IActionResult GetTestTableBody()
    {
        var categories = _context.TestCategories
            .Include(c => c.Discipline)
            .Include(c => c.Tests)
            .OrderBy(tc => tc.Name)
            .ToList();
        return PartialView("_TestTableBody", categories);
    }

    public IActionResult GetQuestionTableBody(int id)
    {
        var test = _context.Tests.Include(t => t.Questions).ThenInclude(q => q.Answers).FirstOrDefault(c => c.Id == id);
        return PartialView("_QuestionTableBody", test);
    }


    public IActionResult CreateTest()
    {
        return PartialView("CreateTest");
    }

    [HttpPost]
    public IActionResult CreateTest(string name, string categoryId, string categoryName, string description)
    {
        if (!string.IsNullOrWhiteSpace(categoryId) && int.TryParse(categoryId, out int parsedId))
        {
            _context.Tests.Add(new Test
            {
                Name = name,
                Description = description,
                TestCategoryId = parsedId,
                CreatedDate = DateTime.Now
            });
            _context.SaveChanges();

            _logger.Log(
                int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0"),
                "Создание теста",
                $"Создан тест: {name} в категории: {categoryName}"
            );
            return Ok();
        }
        else
        {
            return RedirectToAction("Tests");
        }
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
    
    [HttpGet]
    public IActionResult Questions(int testId)
    {
        ViewBag.TestId = testId;
        return PartialView("AddQuestion");
    }

    [HttpPost]
    public IActionResult Questions(int testId, string questionText)
    {
        if (string.IsNullOrWhiteSpace(questionText))
        {
            return BadRequest("Текст вопроса обязателен");
        }
        var testExists = _context.Tests.Any(t => t.Id == testId);
        if (!testExists)
        {
            return NotFound();
        }

        _context.Questions.Add(new Question
        {
            TestId = testId,
            QuestionText = questionText
        });
        _context.SaveChanges();

        _logger.Log(
            int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0"),
            "Добавление вопроса",
            $"Добавлен вопрос в тест {testId}: {questionText}"
        );
        return Ok();
    }
}