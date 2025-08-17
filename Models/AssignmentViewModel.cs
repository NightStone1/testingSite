using System.ComponentModel.DataAnnotations;

namespace testingSite.Models;

public class AssignmentViewModel
{
    public List<Group> Groups { get; set; } = new();
    public List<User> Users { get; set; } = new();
    public List<TestCategory> TestCategories { get; set; } = new();
    public List<Test> Tests { get; set; } = new();

    public List<AssignmentRowViewModel>? Assignments { get; set; }
    public List<GroupAssignmentRowViewModel>? GroupAssignments { get; set; }
}