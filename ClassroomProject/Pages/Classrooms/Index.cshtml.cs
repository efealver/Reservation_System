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
using Microsoft.AspNetCore.Identity;

namespace ClassroomProject.Pages_Classrooms
{   [Authorize(Roles = "Admin,Instructor")]
    public class IndexModel : PageModel
    {
        private readonly ClassroomProject.Data.ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;

        public IndexModel(ClassroomProject.Data.ApplicationDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;

        }

        public IList<Classroom> Classroom { get;set; } = default!;
        public IList<string> CurrentUserRoles { get; set; } = new List<string>();

        public async Task OnGetAsync()
        {
            Classroom = await _context.Classrooms.ToListAsync();
            if (User.Identity.IsAuthenticated)
            {
                var user = await _userManager.GetUserAsync(User);
                if (user != null)
                {
                    CurrentUserRoles = (await _userManager.GetRolesAsync(user)).ToList();
        
                }
            }
        }
    }
}
