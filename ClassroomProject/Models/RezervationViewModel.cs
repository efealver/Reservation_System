namespace ClassroomProject.Models;
using System.ComponentModel.DataAnnotations;
using ClassroomProject.Models;

public class RezervationViewModel
{
  [Required]
    public int ClassroomId { get; set; }

     [Required]
    public int TermId { get; set; }

     [Required]
    public DayOfWeek DayOfWeek { get; set; }

    [Required]
    public TimeSpan StartTime { get; set; }

    [Required]
    public TimeSpan EndTime { get; set; }
    public ReservationStatus Status { get; set; } = ReservationStatus.Pending;

}
