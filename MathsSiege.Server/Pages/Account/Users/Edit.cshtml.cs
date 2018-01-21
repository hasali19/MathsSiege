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
        private readonly IUserRepository userRepository;

        [BindProperty]
        public User UserModel { get; set; }

        public EditModel(IUserRepository userRepository)
        {
            this.userRepository = userRepository;
        }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            UserModel = await userRepository.GetUserAsync(id);

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
            
            UserModel.Password = string.IsNullOrEmpty(UserModel.Password)
                ? null
                : BCryptHasher.HashPassword(UserModel.Password);

            await userRepository.UpdateUserAsync(UserModel.Id, UserModel);

            ViewData["UpdateSuccess"] = true;

            return Page();
        }
    }
}