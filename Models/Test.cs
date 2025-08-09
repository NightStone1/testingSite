using System.ComponentModel.DataAnnotations;

namespace testingSite.Models;

public class Test : ISoftDeletable
{
    public int Id { get; set; }

    public int TestCategoryId { get; set; }
    public TestCategory TestCategory { get; set; }

    [Required]
    public string Name { get; set; }

    public string? Description { get; set; }

    public DateTime CreatedDate { get; set; } = DateTime.Now;

    public ICollection<Question>? Questions { get; set; }

    public bool IsDeleted { get; set; } = false; 
}
