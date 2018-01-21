using MathsSiege.Models;
using MathsSiege.Server.Data;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MathsSiege.Server.Pages.Account.Users
{
    public class IndexModel : PageModel
    {
        private readonly IUserRepository userRepository;

        public ICollection<User> Users { get; private set; }

        public IndexModel(IUserRepository userRepository)
        {
            this.userRepository = userRepository;
        }

        public async Task OnGetAsync()
        {
            Users = (await userRepository.GetUsersAsync())
                .Where(user => user.Username != User.Identity.Name)
                .ToList();
        }
    }
}