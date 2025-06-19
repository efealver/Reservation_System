using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using ClassroomProject.Models;

namespace ClassroomProject.Data;
public class ApplicationDbContext : IdentityDbContext<User>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Term> Terms { get; set; }
    public DbSet<Classroom> Classrooms { get; set; }
    public DbSet<Reservation> Reservations { get; set; }
    public DbSet<Feedback> Feedbacks { get; set; }
    public DbSet<Log> Logs { get; set; }
    public DbSet<Holiday> Holidays { get; set; }
}
