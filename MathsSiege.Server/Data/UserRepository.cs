using MathsSiege.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MathsSiege.Server.Data
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext context;

        public UserRepository(AppDbContext context)
        {
            this.context = context;
        }

        public async Task<ICollection<User>> GetUsersAsync()
        {
            return await context.Users
                .OrderBy(user => user.Username)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<User> GetUserAsync(int id)
        {
            return await context.Users.FindAsync(id);
        }

        public async Task<User> GetUserAsync(string username)
        {
            return await context.Users.FirstOrDefaultAsync(user => user.Username == username);
        }

        public async Task AddUserAsync(User user)
        {
            await context.Users.AddAsync(user);
            await context.SaveChangesAsync();
        }

        public async Task UpdateUserAsync(int id, User updated)
        {
            User user = await GetUserAsync(id);

            if (user != null)
            {
                user.Username = updated.Username ?? user.Username;
                user.Password = updated.Password ?? user.Password;
                user.Role = updated.Role;

                context.Users.Update(user);

                await context.SaveChangesAsync();
            }
        }

        public async Task DeleteUserAsync(int id)
        {
            User user = await GetUserAsync(id);

            if (user != null)
            {
                context.Users.Remove(user);
                await context.SaveChangesAsync();
            }
        }
    }
}
