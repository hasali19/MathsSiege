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
        private readonly IUserRepository userRepository;

        [BindProperty]
        public User UserModel { get; set; }

        public IndexModel(IUserRepository userRepository)
        {
            this.userRepository = userRepository;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            UserModel = await userRepository.GetUserAsync(User.Identity.Name);

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

            UserModel.Password = string.IsNullOrEmpty(UserModel.Password)
                ? null
                : BCryptHasher.HashPassword(UserModel.Password);

            await userRepository.UpdateUserAsync(UserModel.Id, UserModel);

            return Page();
        }
    }
}