using System.ComponentModel.DataAnnotations;
namespace ClassroomProject.Models;
public class Term
{
    public int Id { get; set; }

    [Required]
    public string Name { get; set; } // e.g., "2024-2025 Spring"

    [Required]
    public DateTime StartDate { get; set; }

    [Required]
    public DateTime EndDate { get; set; }

}
