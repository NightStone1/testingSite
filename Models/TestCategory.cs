using System.ComponentModel.DataAnnotations;

namespace testingSite.Models;

public class TestCategory : ISoftDeletable
{
    public int Id { get; set; }

    [Required]
    public string Name { get; set; }

    public int DisciplineId { get; set; }
    
    public DateTime CreatedDate { get; set; } = DateTime.Now;
    
    public Discipline Discipline { get; set; }

    public ICollection<Test>? Tests { get; set; }

    public ICollection<GroupAssignment>? GroupAssignments { get; set; }

    public bool IsDeleted { get; set; } = false; 
}
