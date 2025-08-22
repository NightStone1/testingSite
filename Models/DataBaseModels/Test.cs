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

    // Вопросы
    public ICollection<Question> Questions { get; set; } = new List<Question>();

    // Групповые назначения
    public ICollection<GroupAssignment> GroupAssignments { get; set; } = new List<GroupAssignment>();

    public bool IsDeleted { get; set; } = false; 
}
