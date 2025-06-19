using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ClassroomProject.Data;

namespace ClassroomProject.Pages_Reservations
{
    public class DetailsModel : PageModel
    {
        private readonly ClassroomProject.Data.ApplicationDbContext _context;

        public DetailsModel(ClassroomProject.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public Reservation Reservation { get; set; } = default!;

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

            if (reservation is not null)
            {
                Reservation = reservation;

                return Page();
            }

            return NotFound();
        }
    }
}
