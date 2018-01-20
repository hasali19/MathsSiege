using MathsSiege.Models;
using MathsSiege.Server.Data;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MathsSiege.Server.Pages.Account.Users
{
    public class IndexModel : PageModel
    {
        private readonly AppDbContext context;

        public ICollection<User> Users { get; private set; }

        public IndexModel(AppDbContext context)
        {
            this.context = context;
        }

        public async Task OnGetAsync()
        {
            Users = await context.Users
                .Where(u => u.Username != User.Identity.Name)
                .ToListAsync();
        }
    }
}