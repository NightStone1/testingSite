using System.ComponentModel.DataAnnotations;

namespace testingSite.Models;

public class Attempt : ISoftDeletable
{
    public int Id { get; set; }

    public int AssignmentId { get; set; }

    public Assignment Assignment { get; set; }

    public DateTime StartTime { get; set; } = DateTime.Now;

    public DateTime? EndTime { get; set; }

    public int? Score { get; set; }

    public bool IsPassed { get; set; } = false;

    public ICollection<UserAnswer>? UserAnswers { get; set; }

    public bool IsDeleted { get; set; } = false;    
}
