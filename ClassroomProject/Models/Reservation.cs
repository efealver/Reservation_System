using System.ComponentModel.DataAnnotations;
using ClassroomProject.Models;
public class Reservation
{
    public int Id { get; set; }

    [Required]
    public string UserId { get; set; }
    public User? User { get; set; }  

    [Required]
    public int ClassroomId { get; set; }
    public Classroom? Classroom { get; set; }

    [Required]
    public int TermId { get; set; }
    public Term? Term { get; set; }

    [Required]
    public DayOfWeek DayOfWeek { get; set; }

    [Required]
    public TimeSpan StartTime { get; set; }

    [Required]
    public TimeSpan EndTime { get; set; }

    public ReservationStatus Status { get; set; } = ReservationStatus.Pending;

    public DateTime Date { get; set; }
    public Guid GroupId { get; set; }
}

public enum ReservationStatus
{
    Pending,
    Approved,
    Rejected
}
