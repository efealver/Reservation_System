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
    public class IndexModel : PageModel
    {
        private readonly ClassroomProject.Data.ApplicationDbContext _context;

        public IndexModel(ClassroomProject.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<Log> Log { get;set; } = default!;

        public async Task OnGetAsync()
        {
            Log = await _context.Logs
                .Include(l => l.User).ToListAsync();
        }
    }
}
