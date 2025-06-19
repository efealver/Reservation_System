using System.ComponentModel.DataAnnotations;
namespace ClassroomProject.Models;
public class Classroom
{
    public int Id { get; set; }

    [Required]
    public string Name { get; set; } // e.g., "A101"

    [Range(1, 500)]
    public int Capacity { get; set; }

    public string? Description { get; set; }

    public ICollection<Reservation> Reservations { get; set; }= new List<Reservation>();
    public ICollection<Feedback> Feedbacks { get; set; }= new List<Feedback>();
}
