using System.ComponentModel.DataAnnotations;

namespace testingSite.Models;

public class TestCategory : ISoftDeletable
{
    public int Id { get; set; }

    [Required]
    public string Name { get; set; }

    public int DisciplineId { get; set; }
    public Discipline Discipline { get; set; }

    public DateTime CreatedDate { get; set; } = DateTime.Now;

    public ICollection<Test> Tests { get; set; } = new List<Test>();

    public bool IsDeleted { get; set; } = false; 
}

