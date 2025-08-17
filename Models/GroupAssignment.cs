using System.ComponentModel.DataAnnotations;

namespace testingSite.Models;

public class GroupAssignment : ISoftDeletable
{
    public int Id { get; set; }

    // Группа
    public int GroupId { get; set; }
    public Group Group { get; set; } = null!;

    // Тест или лекция (одно из двух)
    public int? TestId { get; set; }
    public Test? Test { get; set; }

    public int? LectureId { get; set; }
    public Lecture? Lecture { get; set; }

    // Дата и ограничения
    public DateTime AssignedDate { get; set; } = DateTime.Now;
    public int? MaxAttempts { get; set; }

    // Связанные индивидуальные назначения
    public ICollection<Assignment> Assignments { get; set; } = new List<Assignment>();

    // Soft delete
    public bool IsDeleted { get; set; } = false;
}
