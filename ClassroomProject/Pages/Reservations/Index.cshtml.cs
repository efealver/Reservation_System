using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ClassroomProject.Data;
using ClassroomProject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace ClassroomProject.Pages_Reservations
{
    [Authorize(Roles = "Admin,Instructor")]
    public class IndexModel : PageModel
    {
        private readonly ClassroomProject.Data.ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;

        public IndexModel(ClassroomProject.Data.ApplicationDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IList<Reservation> Reservation { get; set; } = default!;
        public HashSet<int> ConflictingReservationIds { get; set; } = new HashSet<int>();
        public IList<string> CurrentUserRoles { get; set; } = new List<string>();


        public async Task OnGetAsync()
        {
            User currentUser = null;

            if (User.Identity.IsAuthenticated)
            {
                currentUser = await _userManager.GetUserAsync(User);
                if (currentUser != null)
                {
                    CurrentUserRoles = (await _userManager.GetRolesAsync(currentUser)).ToList();

                }
            }
            IQueryable<Reservation> query = _context.Reservations
            .Include(r => r.Classroom)
            .Include(r => r.Term)
            .Include(r => r.User);

            if (CurrentUserRoles.Contains("Admin"))
            {
                // Admin sees all reservations
                Reservation = await query
                    .GroupBy(r => r.GroupId)
                    .Select(g => g.First())
                    .ToListAsync();
            }
            else if (CurrentUserRoles.Contains("Instructor") && currentUser != null)
            {
                // Instructor sees only their reservations
                Reservation = await query
                    .Where(r => r.UserId == currentUser.Id)
                    .GroupBy(r => r.GroupId)
                    .Select(g => g.First())
                    .ToListAsync();
            }
            else
            {
                Reservation = new List<Reservation>();
            }


            foreach (var reservation in Reservation)
            {
                var conflicts = Reservation.Where(r =>
                    r.ClassroomId == reservation.ClassroomId &&
                    r.DayOfWeek == reservation.DayOfWeek &&
                    r.TermId == reservation.TermId &&
                    r.Id != reservation.Id &&
                    ((reservation.StartTime >= r.StartTime && reservation.StartTime < r.EndTime) ||
                     (reservation.EndTime > r.StartTime && reservation.EndTime <= r.EndTime) ||
                     (reservation.StartTime <= r.StartTime && reservation.EndTime >= r.EndTime)))
                    .ToList();

                if (conflicts.Any())
                {
                    ConflictingReservationIds.Add(reservation.Id);
                }
            }





        }
    }
}
