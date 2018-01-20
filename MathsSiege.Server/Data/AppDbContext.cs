using MathsSiege.Models;
using Microsoft.EntityFrameworkCore;

namespace MathsSiege.Server.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<Choice> Choices { get; set; }
        public DbSet<GameSession> GameSessions { get; set; }
        public DbSet<Answer> Answers { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var usersBuilder = modelBuilder.Entity<User>();

            usersBuilder
                .HasIndex(u => u.Username)
                .IsUnique();

            usersBuilder
                .Property(u => u.Role)
                .HasDefaultValue(Role.Student);
        }
    }
}
