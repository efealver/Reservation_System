using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using ClassroomProject.Data;
using ClassroomProject.Models;
using Microsoft.AspNetCore.Identity;
using ClassroomProject.Services;

namespace ClassroomProject.Pages_Feedbacks
{
    public class CreateModel : PageModel
    {
        private readonly ClassroomProject.Data.ApplicationDbContext _context;

        private readonly UserManager<User> _userManager;
        private readonly EmailService _emailService;
        public CreateModel(ApplicationDbContext context, UserManager<User> userManager, EmailService emailservice)
        {
            _context = context;
            _userManager = userManager;
            _emailService = emailservice;
        }

        public IActionResult OnGet()
        {
        ViewData["ClassroomId"] = new SelectList(_context.Classrooms, "Id", "Name");
        ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id");
            return Page();
        }

        [BindProperty]
        public FeedbackViewModel FeedbackVM { get; set; } = new FeedbackViewModel();


        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
           var userId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(userId))
            {
                ModelState.AddModelError(string.Empty, "You must be logged in to submit feedback.");
                return Page();
            }

            if (!ModelState.IsValid)
            {
                foreach (var error in ModelState)
                {
                    Console.WriteLine($"Key: {error.Key}");
                    foreach (var err in error.Value.Errors)
                    {
                        Console.WriteLine($"  Error: {err.ErrorMessage}");
                    }
                }

                return Page();
            }

            


             var feedback = new Feedback
                {
                    UserId = userId,
                    ClassroomId = FeedbackVM.ClassroomId,
                    Rating = FeedbackVM.Rating,
                    Comment = FeedbackVM.Comment,
                    CreatedAt = DateTime.UtcNow
                };
            var message = $"User {User.Identity.Name} says :{FeedbackVM.Comment}.";
            await _emailService.SendEmailAsync("alverefe9653@gmail.com", "User Feedback", message);    
            _context.Feedbacks.Add(feedback);
            var affected =await _context.SaveChangesAsync();
            Console.WriteLine("Rows affected: " + affected);
            return RedirectToPage("./Index");
        }
    }
}
