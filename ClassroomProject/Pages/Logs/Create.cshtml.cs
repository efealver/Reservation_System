using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using ClassroomProject.Data;
using ClassroomProject.Models;

namespace ClassroomProject.Pages_Logs
{
    public class CreateModel : PageModel
    {
        private readonly ClassroomProject.Data.ApplicationDbContext _context;

        public CreateModel(ClassroomProject.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
        ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id");
            return Page();
        }

        [BindProperty]
        public Log Log { get; set; } = default!;

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Logs.Add(Log);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
