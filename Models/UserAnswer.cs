using System.ComponentModel.DataAnnotations;

namespace testingSite.Models;

public class UserAnswer
{
    public int Id { get; set; }

    public int AttemptId { get; set; }
    
    public Attempt Attempt { get; set; }

    public int QuestionId { get; set; }

    public Question Question { get; set; }

    public int AnswerId { get; set; }

    public Answer Answer { get; set; }

    public DateTime Timestamp { get; set; } = DateTime.Now;

    public bool IsDeleted { get; set; } = false; 
}
