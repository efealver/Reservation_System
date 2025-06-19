using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ClassroomProject.Data;
using ClassroomProject.Models;

namespace ClassroomProject.Pages_Logs
{
    public class DeleteModel : PageModel
    {
        private readonly ClassroomProject.Data.ApplicationDbContext _context;

        public DeleteModel(ClassroomProject.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Log Log { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var log = await _context.Logs.FirstOrDefaultAsync(m => m.Id == id);

            if (log is not null)
            {
                Log = log;

                return Page();
            }

            return NotFound();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var log = await _context.Logs.FindAsync(id);
            if (log != null)
            {
                Log = log;
                _context.Logs.Remove(Log);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
