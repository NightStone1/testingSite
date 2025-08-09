using System.ComponentModel.DataAnnotations;

namespace testingSite.Models;

public class Question : ISoftDeletable
{
    public int Id { get; set; }

    public int TestId { get; set; }

    public Test Test { get; set; }

    [Required]
    public string QuestionText { get; set; }

    public ICollection<Answer>? Answers { get; set; }

    public ICollection<UserAnswer>? UserAnswers { get; set; }

    public bool IsDeleted { get; set; } = false; 
}
