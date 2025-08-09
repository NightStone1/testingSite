using System.ComponentModel.DataAnnotations;

namespace testingSite.Models;

public class GroupAssignment : ISoftDeletable
{
    public int Id { get; set; }

    public int GroupId { get; set; }
    
    public Group Group { get; set; }

    public int? TestCategoryId { get; set; }

    public TestCategory? TestCategory { get; set; }

    public int? LectureId { get; set; }

    public Lecture? Lecture { get; set; }

    public DateTime AssignedDate { get; set; } = DateTime.Now;

    public int? MaxAttempts { get; set; }

    public ICollection<Assignment>? Assignments { get; set; }

    public bool IsDeleted { get; set; } = false; 
}
