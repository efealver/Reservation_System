using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ClassroomProject.Data;
using ClassroomProject.Models;

namespace ClassroomProject.Pages_Reservations
{
    public class DeleteModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public DeleteModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Reservation Reservation { get; set; } = default!;

        public Guid GroupId { get; private set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reservation = await _context.Reservations
                .Include(r => r.Classroom)
                .Include(r => r.Term)
                .Include(r => r.User)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (reservation == null)
            {
                return NotFound();
            }

            Reservation = reservation;
            GroupId = reservation.GroupId;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            
            var reservation = await _context.Reservations
                .FirstOrDefaultAsync(r => r.Id == id);

            if (reservation == null)
            {
                return NotFound();
            }

            GroupId = reservation.GroupId;

            
            var reservationsInGroup = await _context.Reservations
                .Where(r => r.GroupId == GroupId)
                .ToListAsync();

            _context.Reservations.RemoveRange(reservationsInGroup);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
