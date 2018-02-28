using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MathsSiege.Server.Pages.Account
{
    public class AccessDeniedModel : PageModel
    {
        public IActionResult OnGet(string returnUrl)
        {
            if (string.IsNullOrEmpty(returnUrl) || returnUrl == "/")
            {
                return RedirectToPage("/Account/Index");
            }

            return Page();
        }
    }
}
