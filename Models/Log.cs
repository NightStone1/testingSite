using System.ComponentModel.DataAnnotations;

namespace testingSite.Models;

public class Log
{
    public int Id { get; set; }

    public int UserId { get; set; }
    public User User { get; set; }

    [Required]
    [RegularExpression("login|logout|create|update|delete|assign|attempt")]
    public string ActionType { get; set; }

    [Required]
    public string ActionText { get; set; }

    public DateTime Timestamp { get; set; } = DateTime.Now;
}
