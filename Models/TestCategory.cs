using System.ComponentModel.DataAnnotations;

namespace testingSite.Models;

public class TestCategory
{
    public int Id { get; set; }

    [Required]
    public string Name { get; set; }

    public int DisciplineId { get; set; }
    public Discipline Discipline { get; set; }

    public ICollection<Test>? Tests { get; set; }
    public ICollection<GroupAssignment>? GroupAssignments { get; set; }
}
