namespace testingSite.Models.DTOs;
public class TestDto

{
    public int AssignmentId { get; set; }
    public int TestId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string TestCategory { get; set; } = string.Empty;
    public int AttemptsLeft { get; set; }
    public int? MaxAttempts { get; set; }
    public bool IsAttemptStart { get; set; }
    public List<QuestionDto> Questions { get; set; } = new();
}

public class QuestionDto
{
    public int QuestionId { get; set; }
    public string Text { get; set; } = string.Empty;
    public bool IsMultiple { get; set; }
    public List<AnswerDto> Answers { get; set; } = new();
}

public class AnswerDto
{
    public int AnswerId { get; set; }
    public string Text { get; set; } = string.Empty;
}