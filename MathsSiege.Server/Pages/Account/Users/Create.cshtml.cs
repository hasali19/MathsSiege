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
        private readonly IUserRepository userRepository;

        [BindProperty]
        public User UserModel { get; set; }

        public CreateModel(IUserRepository userRepository)
        {
            this.userRepository = userRepository;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            UserModel.Password = BCryptHasher.HashPassword(UserModel.Password);

            await userRepository.AddUserAsync(UserModel);

            return RedirectToPage("/Account/Users/Edit", new { UserModel.Id });
        }
    }
}