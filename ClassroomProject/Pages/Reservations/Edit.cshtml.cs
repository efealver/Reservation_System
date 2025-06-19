using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ClassroomProject.Data;
using ClassroomProject.Models;
using Microsoft.AspNetCore.Identity;
using ClassroomProject.Services;

namespace ClassroomProject.Pages_Reservations
{
    public class EditModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;

        public EditModel(ApplicationDbContext context, UserManager<User> userManager,EmailService emailService)
        {
            _context = context;
            _userManager = userManager;
            _emailService = emailService;
        }

        [BindProperty]
        public RezervationViewModel ReservationVM { get; set; } = default!;

        [BindProperty]
        public int ReservationId { get; set; }

        private Guid GroupId;
        private readonly EmailService _emailService; // or inject via constructor


        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
                return NotFound();

            var reservation = await _context.Reservations
            .Include(r => r.Classroom)
            .FirstOrDefaultAsync(m => m.Id == id);
            if (reservation == null)
                return NotFound();

            GroupId = reservation.GroupId;

            ReservationVM = new RezervationViewModel
            {
                ClassroomId = reservation.ClassroomId,
                TermId = reservation.TermId,
                DayOfWeek = reservation.DayOfWeek,
                StartTime = reservation.StartTime,
                EndTime = reservation.EndTime,
                Status = reservation.Status
            };

            ReservationId = reservation.Id;

            await PopulateSelectLists();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(userId))
            {
                ModelState.AddModelError(string.Empty, "You must be logged in to edit a reservation.");
                return Page();
            }

            if (!ModelState.IsValid)
            {
                await PopulateSelectLists();
                return Page();
            }

            var baseReservation = await _context.Reservations
            .Include(r => r.Classroom)
            .FirstOrDefaultAsync(r => r.Id == ReservationId);
            if (baseReservation == null)
                return NotFound();

            GroupId = baseReservation.GroupId;

            
            var reservationsInGroup = await _context.Reservations
                .Where(r => r.GroupId == GroupId)
                .ToListAsync();

            var conflictingApprovedReservations = await _context.Reservations
                .Where(r => r.ClassroomId == ReservationVM.ClassroomId &&
                    r.DayOfWeek == ReservationVM.DayOfWeek &&
                    r.TermId == ReservationVM.TermId &&
                    r.Status == ReservationStatus.Approved && 
                    r.Id != ReservationId && 
                    ((ReservationVM.StartTime >= r.StartTime && ReservationVM.StartTime < r.EndTime) ||
                     (ReservationVM.EndTime > r.StartTime && ReservationVM.EndTime <= r.EndTime) ||
                     (ReservationVM.StartTime <= r.StartTime && ReservationVM.EndTime >= r.EndTime)))
        .ToListAsync();

            if (conflictingApprovedReservations.Any() && ReservationVM.Status == ReservationStatus.Approved)
            {
                ModelState.AddModelError(string.Empty, "This reservation overlaps with an already approved reservation. It cannot be approved.");
                await PopulateSelectLists();
                return Page();
            }



            foreach (var res in reservationsInGroup)
            {
                res.ClassroomId = ReservationVM.ClassroomId;
                res.TermId = ReservationVM.TermId;
                res.DayOfWeek = ReservationVM.DayOfWeek;
                res.StartTime = ReservationVM.StartTime;
                res.EndTime = ReservationVM.EndTime;
                res.Status = ReservationVM.Status;
                
            }

            await _context.SaveChangesAsync();
            var classroom = await _context.Classrooms.FirstOrDefaultAsync(c => c.Id == ReservationVM.ClassroomId);
            var user = await _userManager.FindByIdAsync(baseReservation.UserId);
            if (user != null)
            {
                var subject = $"Reservation {ReservationVM.Status}";
                var message = $"Your reservation on {baseReservation.DayOfWeek}s from {baseReservation.StartTime} to {baseReservation.EndTime} in classroom {baseReservation.Classroom.Name} has been {baseReservation.Status}.";

                if (baseReservation.Status == ReservationStatus.Rejected)
                    message += "\n\nReason: Please check with the admin.";

                await _emailService.SendEmailAsync(user.Email, subject, message);
            }

            return RedirectToPage("./Index");
        }

        private async Task PopulateSelectLists()
        {
            ViewData["ClassroomId"] = new SelectList(_context.Classrooms, "Id", "Name");
            ViewData["TermId"] = new SelectList(_context.Terms, "Id", "Name");

            ViewData["DayOfWeekList"] = Enum.GetValues(typeof(DayOfWeek))
                .Cast<DayOfWeek>()
                .Select(d => new SelectListItem
                {
                    Value = ((int)d).ToString(),
                    Text = d.ToString()
                });

            ViewData["StatusList"] = Enum.GetValues(typeof(ReservationStatus))
                .Cast<ReservationStatus>()
                .Select(s => new SelectListItem
                {
                    Value = ((int)s).ToString(),
                    Text = s.ToString()
                });

            await Task.CompletedTask;
        }

        private bool ReservationExists(int id)
        {
            return _context.Reservations.Any(e => e.Id == id);
        }
    }
}
