using MathsSiege.Models;
using System.Collections.Generic;
using System.Linq;

using BCryptHasher = BCrypt.Net.BCrypt;

namespace MathsSiege.Server.Data
{
    public static class DbSeeder
    {
        public static void SeedAll(AppDbContext context)
        {
            SeedUsers(context);
        }

        public static void SeedUsers(AppDbContext context)
        {
            if (context.Users.Any())
            {
                return;
            }

            ICollection<User> seedUsers = GetSeedUsers();

            context.Users.AddRange(seedUsers);
            context.SaveChanges();
        }

        public static IList<User> GetSeedUsers()
        {
            string password1 = BCryptHasher.HashPassword("fy=3G.rf");
            string password2 = BCryptHasher.HashPassword("C]gB5n]h");
            string password3 = BCryptHasher.HashPassword("^9W:W/c^");

            IList<User> users = new List<User>
            {
                new User { Username = "admin", Password = password1, Role = Role.Admin },
                new User { Username = "student_1", Password = password2 },
                new User { Username = "student_2", Password = password3 }
            };

            return users;
        }
    }
}
