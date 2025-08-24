using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using testingSite.Models;
using testingSite.Models.SupportingModels;
using testingSite.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Threading.Tasks;

namespace testingSite.Controllers;

[Authorize(Roles = "teacher")]
public class TeacherController : Controller
{
    private readonly IAppLogger _logger;

    private readonly AppDbContext _context;

    private readonly ISoftDeleteService _softDeleteService;

    public TeacherController(AppDbContext context, IAppLogger logger, ISoftDeleteService softDeleteService)
    {
        _context = context;
        _logger = logger;
        _softDeleteService = softDeleteService;
    }

    public async Task<IActionResult> Dashboard()
    {
        return View();
    }

    public async Task<IActionResult> Lectures()
    {
        return PartialView("Lectures");
    }

    [HttpPost]
    public async Task<IActionResult> DeleteCategory(int id)
    {
        var success = await _softDeleteService.SoftDeleteCategoryAsync(id);

        if (!success)
            return NotFound();

        TempData["Message"] = "Категория удалена (soft delete).";
        return Ok();
    }

    [HttpPost]
    public async Task<IActionResult> DeleteQuestion(int id)
    {
        var success = await _softDeleteService.SoftDeleteQuestionAsync(id);

        if (!success)
            return NotFound();

        TempData["Message"] = "Вопрос удален (soft delete).";
        return Ok();
    }

    [HttpPost]
    public async Task<IActionResult> DeleteTest(int id)
    {
        var success = await _softDeleteService.SoftDeleteTestAsync(id);

        if (!success)
            return NotFound();

        TempData["Message"] = "Тест удалён (soft delete).";
        return Ok();
    }

    public async Task<IActionResult> Tests()
    {
        var testCategories = await _context.TestCategories.Include(tc => tc.Tests).Include(tc => tc.Discipline).OrderBy(tc => tc.Name).ToListAsync();
        return PartialView("Tests", testCategories);
    }

    public async Task<IActionResult> EditTestCategory(int id)
    {
        var category = await _context.TestCategories.FirstOrDefaultAsync(tc => tc.Id == id);
        if (category == null)
            return NotFound();

        return PartialView("EditTestCategory", category);
    }

    public async Task<IActionResult> EditTest(int id)
    {
        var test = await _context.Tests.Include(t => t.Questions).ThenInclude(q => q.Answers).FirstOrDefaultAsync(c => c.Id == id);
        if (test == null)
            return NotFound();

        return PartialView("EditTest", test);
    }

    public async Task<IActionResult> EditQuestion(int Id)
    {
        var question = await _context.Questions.Include(q => q.Answers).FirstOrDefaultAsync(c => c.Id == Id);
        if (question == null)
            return NotFound();
        return PartialView("EditQuestion", question);
    }
    [HttpPost]
    public async Task<IActionResult> EditQuestion(int Id, string questionText)
    {
        var question = await _context.Questions.Include(q => q.Answers).Include(q => q.Test).FirstOrDefaultAsync(c => c.Id == Id);
        if (question == null)
            return NotFound();
        var oldQuestionText = question.QuestionText;
        if (oldQuestionText != questionText)
        {
            question.QuestionText = questionText;
            await _context.SaveChangesAsync();
            _logger.Log(
                int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0"),
                "Изменение вопроса",
                $"Изменен текст вопроса с {oldQuestionText} на {questionText} в тесте {question.Test.Name}"
        );
        }
        return Ok();
    }

    [HttpPost]
    public async Task<IActionResult> EditTestCategory(string name, int id)
    {
        var category = await _context.TestCategories.Include(tc => tc.Discipline).FirstOrDefaultAsync(tc => tc.Id == id);
        if (category == null)
            return NotFound();

        var oldName = category.Name;
        if (oldName != name)
        {
            category.Name = name;
            await _context.SaveChangesAsync();
            _logger.Log(
                int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0"),
                "Изменение категории",
                $"Изменено имя категории с {oldName} на {name} в дисциплине {category.Discipline.Name}"
        );
        }
        return Ok();
    }

    [HttpPost]
    public async Task<IActionResult> EditTest(string name, string description, int id)
    {
        var test = await _context.Tests.Include(t => t.TestCategory).FirstOrDefaultAsync(t => t.Id == id);
        if (test == null)
            return NotFound();

        var oldName = test.Name;
        var oldDescription = test.Description;
        if (oldName != name)
        {
            test.Name = name;
            await _context.SaveChangesAsync();
            _logger.Log(
            int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0"),
            "Изменение Теста",
            $"Изменено имя теста с '{oldName}' на '{name}' в категории '{test.TestCategory}'"
        );
        }
        if (oldDescription != description)
        {
            test.Description = description;
            await _context.SaveChangesAsync();
            _logger.Log(
            int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0"),
            "Изменение Теста",
            $"Изменено описание теста с '{oldDescription}' на '{description}' в категории '{test.TestCategory}'");
        }        
        return Ok();
    }


    public async Task<IActionResult> CreateTestCategory()
    {
        var disciplines = await _context.Disciplines.OrderBy(d => d.Name).ToListAsync();
        return PartialView("CreateTestCategory", disciplines);
    }

    [HttpPost]
    public async Task<IActionResult> CreateTestCategory(string name, string disciplineId, string disciplineName)
    {
        if (!string.IsNullOrWhiteSpace(disciplineId) && int.TryParse(disciplineId, out int parsedId))
        {
            _context.TestCategories.Add(new TestCategory
            {
                Name = name,
                DisciplineId = parsedId
            });
            await _context.SaveChangesAsync();
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
    public async Task<IActionResult> GetTestTableBody()
    {
        var categories = await _context.TestCategories
            .Include(c => c.Discipline)
            .Include(c => c.Tests)
            .OrderBy(tc => tc.Name)
            .ToListAsync();
        return PartialView("_TestTableBody", categories);
    }

    public IActionResult GetQuestionTableBody(int id)
    {
        var test = _context.Tests.Include(t => t.Questions).ThenInclude(q => q.Answers).FirstOrDefault(c => c.Id == id);
        return PartialView("_QuestionTableBody", test);
    }

    public async Task<IActionResult> GetSingleAssignmentsTable()
    {
        return PartialView("_TestSingleAssignmentsTableBody", SingleAssignmentRowViewModel());
    }

    public async Task<IActionResult> GetGroupAssignmentsTable()
    {
        return PartialView("_TestGroupAssignmentsTableBody", GroupAssignmentRowViewModel());
    }

    [HttpGet]
    public async Task<IActionResult> GetStudentsByGroup(int groupId)
    {
        var students = await _context.Users
            .Where(u => u.GroupId == groupId)
            .Select(u => new 
            {
                id = u.Id,
                name = u.Username
            })
            .ToListAsync();

        return Json(students);
    }

    [HttpGet]
    public async Task<IActionResult> GetTestsByCategories(int categoryId)
    {
        var tests = await _context.Tests
            .Where(u => u.TestCategoryId == categoryId)
            .Select(u => new 
            {
                id = u.Id,
                name = u.Name
            })
            .ToListAsync();

        return Json(tests);
    }



    public async Task<IActionResult> CreateTest()
    {
        return PartialView("CreateTest");
    }

    [HttpPost]
    public async Task<IActionResult> CreateTest(string name, string categoryId, string categoryName, string description)
    {
        if (!string.IsNullOrWhiteSpace(categoryId) && int.TryParse(categoryId, out int parsedId))
        {
            await _context.Tests.AddAsync(new Test
            {
                Name = name,
                Description = description,
                TestCategoryId = parsedId,
                CreatedDate = DateTime.Now
            });
            await _context.SaveChangesAsync();

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


    public async Task<IActionResult> Progress()
    {
        return PartialView("Progress");
    }

    public async Task<IActionResult> LecturesAssignments()
    {
        return PartialView("LecturesAssignments");
    }

    public List<GroupAssignmentRowViewModel> GroupAssignmentRowViewModel()
    {
        var groupAssignments = _context.GroupAssignments
            .Include(ga => ga.Group)
            .Include(ga => ga.Test)
                .ThenInclude(t => t.TestCategory)
            .Include(ga => ga.Assignments)
                .ThenInclude(a => a.User)
            .Select(ga => new GroupAssignmentRowViewModel
            {
                GroupAssignmentId = ga.Id,
                GroupId = ga.Group.Id,
                GroupName = ga.Group.Name,
                TestId = ga.Test.Id,
                TestName = ga.Test.Name,
                TestCategory = ga.Test.TestCategory.Name,
                TestCategoryId = ga.Test.TestCategoryId,
                AssignedDate = ga.AssignedDate,
                MaxAttempts = ga.MaxAttempts,
                Assignments = ga.Assignments
                    .Where(a => !a.IsDeleted)
                    .Select(a => new AssignmentRowViewModel
                    {
                        AssignmentId = a.Id,
                        UserId = a.User.Id,
                        UserName = a.User.Username,
                        IsCompleted = a.IsCompleted,
                        AttemptsCount = a.Attempts.Count,
                        AssignedDate = a.AssignedDate,
                        MaxAttempts = a.MaxAttempts,
                        GroupAssignmentId = a.GroupAssignmentId
                    })
                    .ToList()
            })
            .ToList();
        return groupAssignments;
    }

    public List<AssignmentRowViewModel> SingleAssignmentRowViewModel()
    {
        var singleAssignments = _context.Assignments
            .Where(a => !a.IsDeleted && a.GroupAssignmentId == null)
            .Include(a => a.User).ThenInclude(u => u.Group)
            .Include(a => a.Test).ThenInclude(t => t.TestCategory)
            .Select(a => new AssignmentRowViewModel
            {
                AssignmentId = a.Id,
                GroupId = a.User.Group.Id,
                GroupName = a.User.Group.Name,
                UserId = a.User.Id,
                UserName = a.User.Username,
                TestId = a.Test.Id,
                TestName = a.Test.Name,
                TestCategory = a.Test.TestCategory.Name,
                TestCategoryId = a.Test.TestCategoryId,
                IsCompleted = a.IsCompleted,
                AttemptsCount = a.Attempts.Count,
                AssignedDate = a.AssignedDate,
                MaxAttempts = a.MaxAttempts
            })
            .ToList();
        return singleAssignments;
    }
    
    public async Task<IActionResult> TestsAssignments()
    {

        var model = new AssignmentViewModel
        {
            Groups = _context.Groups.ToList(),
            Users = _context.Users.ToList(),
            TestCategories = _context.TestCategories.ToList(),
            Tests = _context.Tests.ToList(),
            Assignments = SingleAssignmentRowViewModel(),
            GroupAssignments = GroupAssignmentRowViewModel()
        };
        return PartialView("TestsAssignments", model);
    }

    [HttpPost]
    public async Task<IActionResult> TestsAssignments(string assignmentType, int groupSelect, int studentSelect, int testCategory, int test, int countAttempts)
    {
        var now = DateTime.Now;
        if (assignmentType == "group")
        {
            if (studentSelect != 0)
            {
                return BadRequest("Выбирать студента при групповом назначении НЕЛЬЗЯ!");
            }
            var students = _context.Users
                .Where(u => u.GroupId == groupSelect)
                .ToList();
            if (!students.Any())
            {
                return BadRequest("В группе нет студентов!");
            }
            var groupAssignment = new GroupAssignment
            {
                GroupId = groupSelect,
                TestId = test,
                MaxAttempts = countAttempts != 0 ? countAttempts : null,
                AssignedDate = now
            };
            await _context.GroupAssignments.AddAsync(groupAssignment);
            await _context.SaveChangesAsync();

            foreach (var student in students)
            {
                var assignment = new Assignment
                {
                    UserId = student.Id,
                    TestId = test,
                    GroupAssignmentId = groupAssignment.Id, 
                    MaxAttempts = countAttempts != 0 ? countAttempts : null,
                    AssignedDate = now
                };
                await _context.Assignments.AddAsync(assignment);
            }
            await _context.SaveChangesAsync();
            _logger.Log(
                int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0"),
                "Cоздание назначения",
                $"Создано групповое назначение. id Группы: {groupSelect}."
            );            
            return Ok();
        }
        if (assignmentType == "single")
        {
            if (studentSelect == 0)
            {
                return BadRequest("Необходимо выбрать студента!");
            }
            var assignment = new Assignment
            {
                UserId = studentSelect,
                TestId = test,
                MaxAttempts = countAttempts != 0 ? countAttempts : null,
                AssignedDate = now
            };
            await _context.Assignments.AddAsync(assignment);
            await _context.SaveChangesAsync();
            _logger.Log(
                int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0"),
                "Cоздание назначения",
                $"Создано одиночное назначение. id студента: {studentSelect}."
            );
            return Ok();
        }
        return BadRequest("Заполните все обязательные поля.");
    }

    
    [HttpGet]
    public async Task<IActionResult> Questions(int testId)
    {
        ViewBag.TestId = testId;
        return PartialView("AddQuestion");
    }

    [HttpPost]
    public async Task<IActionResult> Questions(int testId, string questionText, List<string> answers, List<bool> isCorrect)
    {
        if (string.IsNullOrWhiteSpace(questionText) || answers == null || answers.Count == 0)
        {
            return BadRequest("Заполните все обязательные поля.");
        }
        var testExists = await _context.Tests.AnyAsync(t => t.Id == testId);
        if (!testExists)
        {
            return NotFound();
        }
        var question = new Question
        {
            TestId = testId,
            QuestionText = questionText,
            Answers = answers.Select((ans, i) => new Answer
            {
                AnswerText = ans,
                IsCorrect = isCorrect != null && i < isCorrect.Count && isCorrect[i]
            }).ToList()
        };
        await _context.Questions.AddAsync(question);
        await _context.SaveChangesAsync();

        _logger.Log(
            int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0"),
            "Добавление вопроса",
            $"Добавлен вопрос в тест {testId}: {questionText}"
        );
        return Ok();
    }
}