using System.ComponentModel.DataAnnotations;

namespace testingSite.Models;

public class Assignment
{
    public int Id { get; set; }

    public int UserId { get; set; }
    public User User { get; set; }

    public int GroupAssignmentId { get; set; }
    public GroupAssignment GroupAssignment { get; set; }

    public bool IsCompleted { get; set; } = false;

    public int? MaxAttempts { get; set; }

    public ICollection<Attempt>? Attempts { get; set; }
}
