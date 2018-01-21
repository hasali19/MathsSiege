using MathsSiege.Models;
using MathsSiege.Server.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

using BCryptHasher = BCrypt.Net.BCrypt;

namespace MathsSiege.Server.Pages.Account
{
    public class LoginModel : PageModel
    {
        private readonly IUserRepository userRepository;

        [BindProperty]
        public User UserModel { get; set; }

        public LoginModel(IUserRepository userRepository)
        {
            this.userRepository = userRepository;
        }

        public IActionResult OnGet()
        {
            // Redirect if the user is already logged in
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToPage("/Index");
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl)
        {
            // Ensure username and password are not empty
            if (string.IsNullOrWhiteSpace(UserModel.Username)
                || string.IsNullOrWhiteSpace(UserModel.Password))
            {
                return Error("The username and password fields cannot be blank.");
            }

            // Search the database for the username
            User user = await userRepository.GetUserAsync(UserModel.Username);

            // Check that the user exists
            if (user == null)
            {
                return Error("There is no user with that username.");
            }

            // Verify the submitted password against the stored hash
            bool isMatch = BCryptHasher.Verify(UserModel.Password, user.Password);

            if (!isMatch)
            {
                return Error("The username or password is incorrect.");
            }

            // Create user's claims
            Claim[] claims = new[]
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role.ToString())
            };

            // Create user's identity
            ClaimsIdentity identity = new ClaimsIdentity(claims, "Password");
            ClaimsPrincipal principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(principal);

            // Redirect to the returnUrl if supplied or homepage if not
            if (string.IsNullOrWhiteSpace(returnUrl))
            {
                return RedirectToPage("/Index");
            }

            return Redirect(returnUrl);
        }

        private IActionResult Error(string message)
        {
            ViewData["ErrorMessage"] = message;
            return Page();
        }
    }
}