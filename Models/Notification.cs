using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace testingSite.Models;

public class Notification
{
    public int Id { get; set; }

    [Required]
    public int UserId { get; set; }

    public User User { get; set; }

    [Required]
    public string Message { get; set; }

    public DateTime CreatedAt { get; set; }

    public bool IsRead { get; set; } = false;
    
    public bool IsDeleted { get; set; } = false;
}
