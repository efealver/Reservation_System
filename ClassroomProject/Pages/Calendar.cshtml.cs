using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using ClassroomProject.Data;
using ClassroomProject.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Services;
using Google.Apis.Auth.OAuth2;
using System.Text.Json;
using ClassroomProject.Services;    

namespace ClassroomProject.Pages
{
    [Authorize]
    public class CalendarModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;
        

        public CalendarModel(ApplicationDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public List<CalendarEvent> CalendarEvents { get; set; } = new();
        public IList<string> CurrentUserRoles = new List<string>() ;
        public async Task OnGetAsync()
        {
             if (User.Identity.IsAuthenticated)
            {
                var user = await _userManager.GetUserAsync(User);
                if (user != null)
                {
                    CurrentUserRoles = (await _userManager.GetRolesAsync(user)).ToList();
        
                }
            }
            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(userId)) return;
           
            var reservations = await _context.Reservations
                .Include(r => r.Classroom)
                .Where(r => r.Status == ReservationStatus.Approved)
                .GroupBy(r => r.GroupId)
                .Select(g => g.First())
                .ToListAsync();
            if (CurrentUserRoles.Contains("Instructor"))
            {
                reservations = await _context.Reservations
                .Include(r => r.Classroom)
                .Where(r => r.UserId == userId && r.Status == ReservationStatus.Approved)
                .GroupBy(r => r.GroupId)
                .Select(g => g.First())
                .ToListAsync();         
            }
            foreach (var res in reservations)
            {
                var term = await _context.Terms.FindAsync(res.TermId);
                if (term == null) continue;

                DateTime current = term.StartDate;
                while (current <= term.EndDate)
                {
                    if ((int)current.DayOfWeek == (int)res.DayOfWeek)
                    {
                        var startDateTime = current.Date + res.StartTime;
                        var endDateTime = current.Date + res.EndTime;

                        CalendarEvents.Add(new CalendarEvent
                        {
                            title = $"Classroom {res.Classroom?.Name}",
                            start = startDateTime.ToString("s"),
                            end = endDateTime.ToString("s"),
                            color = "#27b04b"
                        });
                    }
                    current = current.AddDays(1);
                }
            }

            var holidays = await GetHolidaysAsync();
            foreach (var item in holidays)
            {
                if (!string.IsNullOrEmpty(item.Start?.Date))
                {
                    CalendarEvents.Add(new CalendarEvent
                    {
                        title = item.Summary,
                        start = item.Start.Date,
                        allDay = true,
                        color = "#d3d618"
                    });
                }
            }
        }

        private async Task<IList<Event>> GetHolidaysAsync()
        {
            var calendarId = "en.turkish#holiday@group.v.calendar.google.com";

            GoogleCredential credential = await GoogleCredential
                .FromFileAsync("credentials.json", System.Threading.CancellationToken.None)
                .ConfigureAwait(false);

            var service = new CalendarService(new BaseClientService.Initializer
            {
                HttpClientInitializer = credential.CreateScoped(CalendarService.Scope.CalendarReadonly),
                ApplicationName = "ClassroomProject"
            });

            var request = service.Events.List(calendarId);
            request.TimeMin = DateTime.UtcNow;
            request.TimeMax = DateTime.UtcNow.AddMonths(6);
            request.SingleEvents = true;

            var result = await request.ExecuteAsync();
            return result.Items ?? new List<Event>();
        }
    }
}
