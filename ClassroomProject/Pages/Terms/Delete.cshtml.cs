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

namespace ClassroomProject.Pages_Terms
{
    [Authorize(Roles = "Admin")]

    public class DeleteModel : PageModel
    {
        private readonly ClassroomProject.Data.ApplicationDbContext _context;

        public DeleteModel(ClassroomProject.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Term Term { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var term = await _context.Terms.FirstOrDefaultAsync(m => m.Id == id);

            if (term is not null)
            {
                Term = term;

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

            var term = await _context.Terms.FindAsync(id);
            if (term != null)
            {
                Term = term;
                _context.Terms.Remove(Term);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
