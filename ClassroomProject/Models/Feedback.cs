using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;
namespace ClassroomProject.Models;

public class Feedback
{
    public int Id { get; set; }

   
    [Required]
    public String UserId { get; set; }
    public User? User { get; set; }

    [Required]
    public int ClassroomId { get; set; }
    public Classroom? Classroom { get; set; }

    [Range(1, 5)]
    public int Rating { get; set; }

    public string? Comment { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
