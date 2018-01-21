using MathsSiege.Models;
using MathsSiege.Server.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;

using BCryptHasher = BCrypt.Net.BCrypt;

namespace MathsSiege.Server.Pages.Account.Users
{
    public class EditModel : PageModel
    {
        private readonly AppDbContext context;

        [BindProperty]
        public User UserModel { get; set; }

        public EditModel(AppDbContext context)
        {
            this.context = context;
        }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            UserModel = await context.Users.FindAsync(id);

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
                var modelState = ModelState[$"{nameof(UserModel)}.{nameof(UserModel.Password)}"];
                modelState.Errors.Clear();
                modelState.ValidationState = ModelValidationState.Valid;
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }

            User user = await context.Users.FindAsync(UserModel.Id);

            user.Username = UserModel.Username;
            user.Password = string.IsNullOrEmpty(UserModel.Password)
                ? user.Password
                : BCryptHasher.HashPassword(UserModel.Password);

            await context.SaveChangesAsync();

            return Page();
        }
    }
}