using System.ComponentModel.DataAnnotations;

namespace testingSite.Models;

public class Discipline
{
    public int Id { get; set; }

    [Required]
    public string Name { get; set; }
    
    public string? Content { get; set; }

    public ICollection<TestCategory>? TestCategories { get; set; }

    public ICollection<Lecture>? Lectures { get; set; }

    public bool IsDeleted { get; set; } = false; 
}
