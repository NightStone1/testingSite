using testingSite.Data;
using testingSite.Models;
using Microsoft.EntityFrameworkCore;
public class SoftDeleteService : ISoftDeleteService
{
    private readonly AppDbContext _context;

    public SoftDeleteService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<bool> SoftDeleteCategoryAsync(int categoryId)
    {
        var category = await _context.TestCategories
            .Include(tc => tc.Tests)
                .ThenInclude(t => t.Questions)
                    .ThenInclude(q => q.Answers)
            .FirstOrDefaultAsync(tc => tc.Id == categoryId);
        if (category == null)
            return false;

        category.IsDeleted = true;

        foreach (var test in category.Tests)
        {
            test.IsDeleted = true;
            foreach (var question in test.Questions)
            {
                question.IsDeleted = true;
                foreach (var answer in question.Answers)
                {
                    answer.IsDeleted = true;
                }
            }
        }


        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> SoftDeleteTestAsync(int testId)
    {
        var test = await _context.Tests
            .Include(t => t.Questions)
                .ThenInclude(q => q.Answers)
            .FirstOrDefaultAsync(t => t.Id == testId);

        if (test == null)
            return false;

        test.IsDeleted = true;
        foreach (var question in test.Questions)
        {
            question.IsDeleted = true;
            foreach (var answer in question.Answers)
            {
                answer.IsDeleted = true;
            }
        }

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> SoftDeleteQuestionAsync(int QuestionId)
    {
        var question = await _context.Questions
            .Include(q => q.Answers)
            .FirstOrDefaultAsync(q => q.Id == QuestionId);

        if (question == null)
            return false;

        question.IsDeleted = true;
        foreach (var answer in question.Answers)
        {
            answer.IsDeleted = true;
        }

        await _context.SaveChangesAsync();
        return true;
    }
}
