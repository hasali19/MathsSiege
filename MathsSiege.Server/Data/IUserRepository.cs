using System.Collections.Generic;
using System.Threading.Tasks;
using MathsSiege.Models;

namespace MathsSiege.Server.Data
{
    public interface IUserRepository
    {
        Task AddUserAsync(User user);
        Task DeleteUserAsync(int id);
        Task<User> GetUserAsync(int id);
        Task<User> GetUserAsync(string username);
        Task<ICollection<User>> GetUsersAsync();
        Task UpdateUserAsync(int id, User updated);
    }
}