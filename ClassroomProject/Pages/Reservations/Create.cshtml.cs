using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using ClassroomProject.Data;
using ClassroomProject.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ClassroomProject.Services;

namespace ClassroomProject.Pages_Reservations
{
    public class CreateModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly HolidayChecker _holidayChecker;
        private readonly EmailService _emailService;
        public CreateModel(ApplicationDbContext context, UserManager<User> userManager, HolidayChecker holidayChecker, EmailService emailService)
        {
            _context = context;
            _userManager = userManager;
            _holidayChecker = holidayChecker;
            _emailService = emailService;
        }
        
        public IList<string> CurrentUserRoles { get; set; } = new List<string>();

        public async Task<IActionResult> OnGetAsync()
        {
             if (User.Identity.IsAuthenticated)
            {
                var user = await _userManager.GetUserAsync(User);
                if (user != null)
                {
                    CurrentUserRoles = (await _userManager.GetRolesAsync(user)).ToList();
        
                }
            }
            ViewData["ClassroomId"] = new SelectList(_context.Classrooms, "Id", "Name");
            ViewData["TermId"] = new SelectList(_context.Terms, "Id", "Name");

            ViewData["DayOfWeekList"] = Enum.GetValues(typeof(DayOfWeek))
                 .Cast<DayOfWeek>()
                 .Select(d => new SelectListItem
                 {
                     Value = ((int)d).ToString(),
                     Text = d.ToString()
                 });

            return Page();
        }

        [BindProperty]
        public RezervationViewModel ReservationVM { get; set; } = new RezervationViewModel();

        public async Task<IActionResult> OnPostAsync()
        {
            var userId = _userManager.GetUserId(User);
            if (!ModelState.IsValid || string.IsNullOrEmpty(userId))
            {
                return Page();
            }

            var term = await _context.Terms.FindAsync(ReservationVM.TermId);
            if (term == null)
            {
                ModelState.AddModelError(string.Empty, "Selected term not found.");
                return Page();
            }

            var groupId = Guid.NewGuid();
            var reservations = new List<Reservation>();

            DateTime current = term.StartDate;
            while (current <= term.EndDate)
            {
                if (current.DayOfWeek == ReservationVM.DayOfWeek)
                {
                    // Check if the current date is a holiday
                    if (await _holidayChecker.IsHolidayAsync(current))
                    {
                        var user = await _userManager.GetUserAsync(User);
                        var message = $"Your reservation on {current:dddd, dd MMM yyyy} falls on an official holiday.";
                        await _emailService.SendEmailAsync(user.Email, "Reservation Overlap - Holiday", message);

                        ModelState.AddModelError("", $"Reservation on {current:dddd, dd MMM yyyy} falls on an official holiday.");
                        await PopulateSelectListsAsync();
                        
                    }

                    reservations.Add(new Reservation
                    {
                        GroupId = groupId,
                        UserId = userId,
                        ClassroomId = ReservationVM.ClassroomId,
                        TermId = ReservationVM.TermId,
                        DayOfWeek = ReservationVM.DayOfWeek,
                        Date = current,
                        StartTime = ReservationVM.StartTime,
                        EndTime = ReservationVM.EndTime,
                        Status = ReservationStatus.Pending
                    });
                }

                current = current.AddDays(1);
            }

            var conflicts = await _context.Reservations
                .Where(r => r.ClassroomId == ReservationVM.ClassroomId &&
                            r.DayOfWeek == ReservationVM.DayOfWeek &&
                            r.TermId == ReservationVM.TermId &&
                            ((ReservationVM.StartTime >= r.StartTime && ReservationVM.StartTime < r.EndTime) ||
                             (ReservationVM.EndTime > r.StartTime && ReservationVM.EndTime <= r.EndTime) ||
                             (ReservationVM.StartTime <= r.StartTime && ReservationVM.EndTime >= r.EndTime)))
                .ToListAsync();

            if (conflicts.Any())
            {
                ModelState.AddModelError(string.Empty, "The selected time slot overlaps with an existing reservation.");
                await PopulateSelectListsAsync();
                
            }

            _context.Reservations.AddRange(reservations);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }

        private async Task PopulateSelectListsAsync()
        {
            ViewData["ClassroomId"] = new SelectList(await _context.Classrooms.ToListAsync(), "Id", "Name");
            ViewData["TermId"] = new SelectList(await _context.Terms.ToListAsync(), "Id", "Name");

            ViewData["DayOfWeekList"] = Enum.GetValues(typeof(DayOfWeek))
                .Cast<DayOfWeek>()
                .Select(d => new SelectListItem
                {
                    Value = ((int)d).ToString(),
                    Text = d.ToString()
                });
        }
    }
}
