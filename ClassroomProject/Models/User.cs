namespace ClassroomProject.Models;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

public class User: IdentityUser
{

    [Required]
    public string FullName { get; set; }

    [Required]
    public UserRole Role { get; set; } // Enum: Admin or Instructor

    public ICollection<Reservation> Reservations { get; set; }
    public ICollection<Feedback> Feedbacks { get; set; }
}
public enum UserRole
{
    Admin,
    Instructor
}
