using MathsSiege.Models;
using MathsSiege.Server.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;
using BCryptHasher = BCrypt.Net.BCrypt;

namespace MathsSiege.Server.Pages.Account.Users
{
    public class CreateModel : PageModel
    {
        private readonly AppDbContext context;

        [BindProperty]
        public User UserModel { get; set; }

        public CreateModel(AppDbContext context)
        {
            this.context = context;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            UserModel.Password = BCryptHasher.HashPassword(UserModel.Password);

            await context.Users.AddAsync(UserModel);
            await context.SaveChangesAsync();

            return RedirectToPage("/Account/Users/Edit", new { UserModel.Id });
        }
    }
}