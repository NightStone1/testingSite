public class GroupAssignmentRowViewModel
{
    public int GroupAssignmentId { get; set; }

    // Группа
    public int GroupId { get; set; }
    public string GroupName { get; set; }
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

    // Список индивидуальных назначений внутри этой группы (опционально)
    public List<AssignmentRowViewModel>? Assignments { get; set; }
}