using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ClassroomProject.Models;
using System.Threading.Tasks;

namespace ClassroomProject.Pages_Users
{
    public class CreateModel : PageModel
    {
        private readonly UserManager<User> _userManager;

        public CreateModel(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        [BindProperty]
        public CreateUserViewModel Input { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            var user = new User
            {
                UserName = Input.Email,
                Email = Input.Email,
                FullName = Input.FullName,
                Role = Input.Role
            };

            var result = await _userManager.CreateAsync(user, Input.Password);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, Input.Role.ToString());
                return RedirectToPage("Index");
            }

            foreach (var error in result.Errors)
                ModelState.AddModelError(string.Empty, error.Description);

            return Page();
        }
    }
}
