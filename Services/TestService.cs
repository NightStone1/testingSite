using testingSite.Data;
using testingSite.Models;
using Microsoft.EntityFrameworkCore;
using testingSite.Models.DTOs;
public class TestService : ITestService
{
    private readonly AppDbContext _context;
    private readonly Random _random = new Random();

    public TestService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<TestDto> AssembleTestAsync(int testId, int assignmentId)
    {
        var test = await _context.Tests
            .Include(t => t.TestCategory)
            .Include(t => t.Questions)
                .ThenInclude(q => q.Answers)
            .FirstOrDefaultAsync(t => t.Id == testId);

        if (test == null)
            throw new Exception("Тест не найден");

        var questions = test.Questions
            .Where(q => !q.IsDeleted)
            .OrderBy(x => _random.Next())
            .Select(q => new QuestionDto
            {
                QuestionId = q.Id,
                Text = q.QuestionText,
                IsMultiple = q.Answers.Count(a => !a.IsDeleted && a.IsCorrect) > 1,
                Answers = q.Answers
                    .Where(a => !a.IsDeleted)
                    .OrderBy(x => _random.Next())
                    .Select(a => new AnswerDto
                    {
                        AnswerId = a.Id,
                        Text = a.AnswerText
                    }).ToList()
            }).ToList();
        var assignment = await _context.Assignments.Include(a => a.Attempts).FirstOrDefaultAsync(a => a.Id == assignmentId);
        if (assignment == null)
            throw new Exception("Назначение теста не найдено");

        var attempts = assignment.Attempts ?? new List<Attempt>();
        var finishedAttemptsCount = attempts.Count(a => a.EndTime.HasValue);

        int attemptsLeft = assignment.MaxAttempts.HasValue
            ? Math.Max(0, assignment.MaxAttempts.Value - finishedAttemptsCount)
            : int.MaxValue;

        bool isAttemptStart = attempts.Any(a => !a.EndTime.HasValue);
        return new TestDto
        {
            AssignmentId = assignmentId,
            TestId = test.Id,
            TestCategory = test.TestCategory.Name,
            AttemptsLeft = attemptsLeft,
            MaxAttempts = assignment.MaxAttempts,
            Title = test.Name,
            Description = test.Description,
            Questions = questions,
            IsAttemptStart = isAttemptStart
        };
    }
    public async Task<Attempt?> StartAttemptAsync(int assignmentId)
    {
        var assignment = await _context.Assignments
            .FirstOrDefaultAsync(a => a.Id == assignmentId);

        if (assignment == null)
            return null;

        // 1. Проверяем, есть ли незавершённая попытка
        var existingAttempt = await _context.Attempts
            .Where(a => a.AssignmentId == assignmentId && a.EndTime == null)
            .OrderByDescending(a => a.StartTime)
            .FirstOrDefaultAsync();

        if (existingAttempt != null)
            return existingAttempt;

        // 2. Проверяем лимиты
        var attemptsCount = await _context.Attempts
            .CountAsync(a => a.AssignmentId == assignmentId);

        if (assignment.MaxAttempts != null &&
            assignment.MaxAttempts != 0 &&
            attemptsCount >= assignment.MaxAttempts)
        {
            // Лимит исчерпан
            return null;
        }

        // 3. Создаём новую попытку
        var newAttempt = new Attempt
        {
            AssignmentId = assignmentId,
            StartTime = DateTime.Now
        };

        _context.Attempts.Add(newAttempt);
        await _context.SaveChangesAsync();

        return newAttempt;
    }
    public async Task SaveUserAnswersAsync(TestResultDto result)
    {
        _context.UserAnswers.AddRange(
            result.Answers.SelectMany(ans =>
                ans.SelectedAnswerIds.Select(selected =>
                    new UserAnswer
                    {
                        AttemptId = result.AttemptId,
                        QuestionId = ans.QuestionId,
                        AnswerId = selected,
                        Timestamp = DateTime.Now
                    })));
        await _context.SaveChangesAsync();
    }

    public async Task CompleteAttemptAsync(int attemptId)
    {
        var attempt = await _context.Attempts
            .Include(a => a.Assignment)
                .ThenInclude(ass => ass.Test)
                    .ThenInclude(t => t.Questions)
            .Include(a => a.UserAnswers)
                .ThenInclude(ua => ua.Answer)
            .Include(a => a.UserAnswers)
                .ThenInclude(ua => ua.Question)
                    .ThenInclude(q => q.Answers)
            .FirstOrDefaultAsync(a => a.Id == attemptId);

        if (attempt == null) return;

        int correctQuestions = 0;

        // группируем ответы по вопросам
        var groupedByQuestion = attempt.UserAnswers
            .GroupBy(ua => ua.QuestionId);

        foreach (var group in groupedByQuestion)
        {
            var question = group.First().Question;

            // множество правильных ID
            var correctIds = question.Answers
                .Where(a => a.IsCorrect)
                .Select(a => a.Id)
                .ToHashSet();

            // множество выбранных студентом ID (без null)
            var chosenIds = group
                .Where(ua => ua.AnswerId.HasValue)   // убираем null
                .Select(ua => ua.AnswerId.Value)     // берём значение
                .ToHashSet();

            // зачёт только если совпали множества
            if (correctIds.SetEquals(chosenIds))
                correctQuestions++;
        }

        int totalQuestions = attempt.Assignment.Test.Questions.Count;

        attempt.Score = correctQuestions;
        attempt.IsPassed = correctQuestions >= (int)Math.Ceiling(totalQuestions * 0.6); // >= 60%
        attempt.EndTime = DateTime.Now;
        attempt.Assignment.IsCompleted = attempt.IsPassed;

        await _context.SaveChangesAsync();
    }

}
