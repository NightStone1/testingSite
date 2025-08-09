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
            
            .FirstOrDefaultAsync(tc => tc.Id == categoryId);

        if (category == null)
            return false;

        category.IsDeleted = true;

        foreach (var test in category.Tests)
        {
            test.IsDeleted = true;
        }

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> SoftDeleteTestAsync(int testId)
    {
        var test = await _context.Tests
            .FirstOrDefaultAsync(t => t.Id == testId);

        if (test == null)
            return false;

        test.IsDeleted = true;

        await _context.SaveChangesAsync();
        return true;
    }
}
