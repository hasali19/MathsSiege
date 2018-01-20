using MathsSiege.Models;
using MathsSiege.Server.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Linq;
using System.Threading.Tasks;
using BCryptHasher = BCrypt.Net.BCrypt;

namespace MathsSiege.Server.Pages.Account
{
    public class IndexModel : PageModel
    {
        private readonly AppDbContext context;

        [BindProperty]
        public User UserModel { get; set; }

        public IndexModel(AppDbContext context)
        {
            this.context = context;
        }

        public IActionResult OnGet()
        {
            UserModel = context.Users.FirstOrDefault(u => u.Username == User.Identity.Name);

            if (UserModel == null)
            {
                return NotFound();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (string.IsNullOrEmpty(UserModel.Password))
            {
                ModelState[$"{nameof(UserModel)}.{nameof(UserModel.Password)}"].Errors.Clear();
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }

            User user = await context.Users.FindAsync(UserModel.Id);

            user.Username = UserModel.Username;
            user.Password = BCryptHasher.HashPassword(UserModel.Password);

            await context.SaveChangesAsync();

            return Page();
        }
    }
}