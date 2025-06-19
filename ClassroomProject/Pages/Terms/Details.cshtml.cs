using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ClassroomProject.Data;
using ClassroomProject.Models;

namespace ClassroomProject.Pages_Terms
{
    public class DetailsModel : PageModel
    {
        private readonly ClassroomProject.Data.ApplicationDbContext _context;

        public DetailsModel(ClassroomProject.Data.ApplicationDbContext context)
        {
            _context = context;
        }

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
    }
}
