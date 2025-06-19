namespace ClassroomProject.Models;
using System.ComponentModel.DataAnnotations;
using ClassroomProject.Models;

public class CreateUserViewModel
{
    [Required]
    public string FullName { get; set; }

    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    public UserRole Role { get; set; }

    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; }
}
