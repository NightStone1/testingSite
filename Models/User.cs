using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace testingSite.Models;

public class User
{
    public int Id { get; set; }

    [Required, StringLength(50, MinimumLength = 3)]
    public string Username { get; set; }

    [Required, MinLength(60)]
    public string PasswordHash { get; set; }

    [Required]
    [RegularExpression("student|teacher|admin")]
    public string Role { get; set; }

    public int? GroupId { get; set; }
    public Group? Group { get; set; }

    public ICollection<Lecture>? CreatedLectures { get; set; }
    public ICollection<Assignment>? Assignments { get; set; }
    public ICollection<Log>? Logs { get; set; }
}
