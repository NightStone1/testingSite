using System.ComponentModel.DataAnnotations;

namespace testingSite.Models;

public class Group : ISoftDeletable
{
    public int Id { get; set; }

    [Required]
    public string Name { get; set; }

    public ICollection<User>? Users { get; set; }
    
    public ICollection<GroupAssignment>? Assignments { get; set; }

    public bool IsDeleted { get; set; } = false; 
}
