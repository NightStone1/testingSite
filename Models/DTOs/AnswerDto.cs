namespace testingSite.Models.DTOs;
public class UserAnswerDto
{
    public int QuestionId { get; set; }
    public List<int> SelectedAnswerIds { get; set; } = new();
}

public class TestResultDto
{
    public int AttemptId { get; set; }
    public List<UserAnswerDto>? Answers { get; set; } = new();
}
