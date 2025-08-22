using testingSite.Models;
using testingSite.Models.DTOs;
public interface ITestService
{
    Task<TestDto> AssembleTestAsync(int testId, int assignmentId);

    Task<Attempt> StartAttemptAsync(int assignmentId);
    Task SaveUserAnswersAsync(TestResultDto result);
    Task CompleteAttemptAsync(int attemptId);
}