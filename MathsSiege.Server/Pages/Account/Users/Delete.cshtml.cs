using MathsSiege.Server.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;

namespace MathsSiege.Server.Pages.Account.Users
{
    public class DeleteModel : PageModel
    {
        private IUserRepository userRepository;

        public DeleteModel(IUserRepository userRepository)
        {
            this.userRepository = userRepository;
        }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var user = await userRepository.GetUserAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            await userRepository.DeleteUserAsync(id);

            return RedirectToPage("/Account/Users/Index");
        }
    }
}