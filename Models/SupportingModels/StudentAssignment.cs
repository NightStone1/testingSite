namespace testingSite.Models.SupportingModels;

public class StudentAssignment
{
    public int AssignmentId { get; set; }
    // Тестовая категория
    public int TestCategoryId { get; set; }
    public string TestCategory { get; set; }
    // Тест
    public int TestId { get; set; }
    public string TestName { get; set; }
    // Прочее
    public int? MaxAttempts { get; set; }
    public int? AttemptsCount { get; set; }
    public bool IsCompleted { get; set; }
    public DateTime AssignedDate { get; set; }
    public int? Result { get; set; }
    
}