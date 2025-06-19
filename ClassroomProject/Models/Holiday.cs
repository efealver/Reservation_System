using System.ComponentModel.DataAnnotations;
namespace ClassroomProject.Models;
public class Holiday
{
    public int Id { get; set; }

    [Required]
    public DateTime Date { get; set; }

    [Required]
    public string Description { get; set; }
}
