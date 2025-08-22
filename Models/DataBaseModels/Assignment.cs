using System.ComponentModel.DataAnnotations;

namespace testingSite.Models;

public class Assignment : ISoftDeletable
{
    public int Id { get; set; }

    // Пользователь
    public int UserId { get; set; }
    public User User { get; set; }

    // Групповое назначение (опционально)
    public int? GroupAssignmentId { get; set; }
    public GroupAssignment? GroupAssignment { get; set; }

    // Для одиночного назначения напрямую на тест или лекцию
    public int? TestId { get; set; }
    public Test? Test { get; set; }

    public int? LectureId { get; set; }
    public Lecture? Lecture { get; set; }

    // Статус и дата
    public DateTime AssignedDate { get; set; } = DateTime.Now;
    public bool IsCompleted { get; set; } = false;
    public int? MaxAttempts { get; set; }

    // Попытки
    public ICollection<Attempt> Attempts { get; set; } = new List<Attempt>();

    // Soft delete
    public bool IsDeleted { get; set; } = false;
}
