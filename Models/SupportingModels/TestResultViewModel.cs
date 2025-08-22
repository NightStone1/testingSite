namespace testingSite.Models.SupportingModels;
public class TestResultViewModel
{
    public int AttemptId { get; set; }
    public string TestTitle { get; set; } = string.Empty;
    public int? Score { get; set; }
    public int TotalQuestions { get; set; }
    public int Percent { get; set; }
    public bool IsPassed { get; set; }
    public DateTime EndTime { get; set; }
}