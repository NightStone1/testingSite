using System.ComponentModel.DataAnnotations;

namespace testingSite.Models;

public class Lecture
{
    public int Id { get; set; }

    public int DisciplineId { get; set; }

    public Discipline Discipline { get; set; }

    [Required]
    public string Title { get; set; }

    [Required]
    public string Content { get; set; }

    public int? OrderIndex { get; set; }

    public int? CreatedBy { get; set; }

    public User? Creator { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public ICollection<GroupAssignment>? GroupAssignments { get; set; }

    public bool IsDeleted { get; set; } = false; 
}
