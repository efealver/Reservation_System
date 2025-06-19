using System.ComponentModel.DataAnnotations;
namespace ClassroomProject.Models;
public class Log
{
    public int Id { get; set; }

    public String? UserId { get; set; } // Nullable for unauthenticated access
    public User User { get; set; }

    [Required]
    public string Action { get; set; } 

    public string Description { get; set; }

    public bool IsSuccess { get; set; }

    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}
