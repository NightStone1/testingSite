using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using testingSite.Models;
using testingSite.Data;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using testingSite.Models.SupportingModels;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using testingSite.Models.DTOs;

namespace testingSite.Controllers;

[Authorize(Roles = "student")]
public class StudentController : Controller
{
    private readonly IAppLogger _logger;

    private readonly AppDbContext _context;

    private readonly ITestService _testAssemblyService;

    public StudentController(AppDbContext context, IAppLogger logger, ITestService testAssemblyService)
    {
        _testAssemblyService = testAssemblyService;
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

    
    public List <StudentAssignment> GetTestsByUser()
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
        var assignments = _context.Assignments
            .Include(a => a.Test)
                .ThenInclude(t => t.TestCategory)
            .Include(a => a.Attempts)
            .Where(a => a.UserId == userId)
            .ToList();

        var tests = assignments.Select(a => new StudentAssignment
        {
            AssignmentId = a.Id,
            IsCompleted = a.IsCompleted,
            AttemptsCount = a.Attempts.Count,
            AssignedDate = a.AssignedDate,
            MaxAttempts = a.MaxAttempts,
            TestCategoryId = a.Test.TestCategoryId,
            TestCategory = a.Test.TestCategory.Name,
            TestId = a.TestId.GetValueOrDefault(),
            TestName = a.Test.Name,
            Result = a.Attempts
                .OrderByDescending(a => a.Score)
                .FirstOrDefault()?.Score
        }).ToList();

        return tests;
    }

    public IActionResult Tests()
    {
        return PartialView("Tests", GetTestsByUser());
    }
    
    public IActionResult GetTestTable()
    {
        return PartialView("_TestTable", GetTestsByUser());
    }
    
    
    [HttpGet]
    public async Task<IActionResult> TakeTest(int id)
    {        
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        try
        {
            
            var assignment = _context.Assignments.Include(a=>a.Attempts).FirstOrDefault(a => a.Id == id && a.UserId == userId);
            if (!assignment.IsCompleted)
            {
                var testId = assignment?.TestId.GetValueOrDefault() ?? 0;
                var test = await _testAssemblyService.AssembleTestAsync(testId, id);
                return PartialView(test);
            }
            var attempt = assignment.Attempts.OrderByDescending(a => a.Score).FirstOrDefault();
            if (attempt == null)
            {
                return NotFound("Попытка не найдена для завершенного теста.");
            }
            return RedirectToAction("Result", new { attemptId = attempt.Id });
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = "Тест не найден или произошла ошибка.";
            return RedirectToAction("Tests");
        }
    }
    [HttpPost]
    public async Task<IActionResult> StartTest(int assignmentId)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!); 
        var assignment = _context.Assignments
            .FirstOrDefault(a => a.Id == assignmentId && a.UserId == userId);

        if (assignment == null)
            return NotFound();

        var attempt = await _testAssemblyService.StartAttemptAsync(assignment.Id);

            if (attempt == null)
            {
                // лимит попыток исчерпан
                TempData["ErrorMessage"] = "У вас больше нет доступных попыток.";
                return RedirectToAction("TakeTest", new { id = assignment.Id }); 
            }

            // редирект сразу на страницу прохождения теста
            return RedirectToAction("Test", new { attemptId = attempt.Id }); 
    }

    [HttpPost]
    public async Task<IActionResult> EndTest(int assignmentId, Dictionary<int, List<int>> Answers)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var assignment = _context.Assignments
            .FirstOrDefault(a => a.Id == assignmentId && a.UserId == userId);
        if (assignment == null)
            return NotFound();
        var attempt = await _context.Attempts
        .FirstOrDefaultAsync(a => a.AssignmentId == assignmentId && a.EndTime == null);
        if (attempt == null)
        {
            return NotFound("Попытка не найдена или уже завершена.");
        }
        
        // Получаем все вопросы теста (чтобы знать даже те, на которые не ответили)
        var questions = _context.Questions
            .Where(q => q.TestId == assignment.TestId)
            .Select(q => q.Id)
            .ToList();

        var userAnswerDtos = questions.Select(qId => new UserAnswerDto
        {
            QuestionId = qId,
            SelectedAnswerIds = (Answers != null && Answers.ContainsKey(qId) && Answers[qId] != null)
                ? Answers[qId]
                : new List<int>() // пусто, если не ответил
        }).ToList();

        var testResultDto = new TestResultDto
        {
            AttemptId = attempt.Id,
            Answers = userAnswerDtos
        };
        await _testAssemblyService.SaveUserAnswersAsync(testResultDto);
        await _testAssemblyService.CompleteAttemptAsync(attempt.Id);
        return RedirectToAction("Result", new { attemptId = attempt.Id });
    }
    [HttpGet]
    public async Task<IActionResult> Result(int attemptId)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var attempt = await _context.Attempts
            .Include(a => a.Assignment)
                .ThenInclude(ass => ass.Test)
                    .ThenInclude(t => t.Questions)
            .Include(a => a.UserAnswers)
                .ThenInclude(ua => ua.Answer)
            .Include(a => a.UserAnswers)
                .ThenInclude(ua => ua.Question)
            .FirstOrDefaultAsync(a => a.Id == attemptId && a.Assignment.UserId == userId);

        if (attempt == null)
            return NotFound();

        int totalQuestions = attempt.Assignment.Test.Questions.Count;

        var viewModel = new TestResultViewModel
        {
            AttemptId = attempt.Id,
            TestTitle = attempt.Assignment.Test.Name,
            Score = attempt.Score,
            TotalQuestions = totalQuestions,
            Percent = totalQuestions > 0 ? (int)Math.Round((double)attempt.Score / totalQuestions * 100) : 0,
            IsPassed = attempt.IsPassed,
            EndTime = attempt.EndTime!.Value
        };

        return View(viewModel);
    }


    [HttpGet]
    public async Task<IActionResult> Test(int attemptId)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var attempt = await _context.Attempts
            .Include(a => a.Assignment)
            .FirstOrDefaultAsync(a => a.Id == attemptId && a.Assignment.UserId == userId);
        if (attempt == null)
            return NotFound();        
        try
        {
            var testId = (await _context.Assignments.FirstOrDefaultAsync(a => a.Id == attempt.AssignmentId && a.UserId == userId))?.TestId.GetValueOrDefault() ?? 0;
            var test = await _testAssemblyService.AssembleTestAsync(testId, attempt.AssignmentId);
            return PartialView(test);
        }
        catch (Exception ex)
        {
            return BadRequest($"Тест не найден");
        }
    }


    public IActionResult Progress()
    {
        return PartialView("Progress");
    }
}