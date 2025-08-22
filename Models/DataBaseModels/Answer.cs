using System.ComponentModel.DataAnnotations;

namespace testingSite.Models;

public class Answer : ISoftDeletable
{
    public int Id { get; set; }

    public int QuestionId { get; set; }
    
    public Question Question { get; set; }

    [Required]
    public string AnswerText { get; set; }

    public bool IsCorrect { get; set; } = false;

    public ICollection<UserAnswer>? UserAnswers { get; set; }
    
    public bool IsDeleted { get; set; } = false;
}
