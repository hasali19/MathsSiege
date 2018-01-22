using MathsSiege.Models;
using System;
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
            SeedQuestions(context);
            SeedGameSessions(context);
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

        public static void SeedQuestions(AppDbContext context)
        {
            if (context.Questions.Any())
            {
                return;
            }

            ICollection<Question> seedQuestions = GetSeedQuestions();

            context.Questions.AddRange(seedQuestions);
            context.SaveChanges();
        }

        public static void SeedGameSessions(AppDbContext context)
        {
            if (context.GameSessions.Any())
            {
                return;
            }

            ICollection<GameSession> seedGameSessions = GetSeedGameSessions();

            context.GameSessions.AddRange(seedGameSessions);
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

        public static IList<Question> GetSeedQuestions()
        {
            return new List<Question>
            {
                new Question
                {
                    Text = "What is 1 + 1?",
                    Difficulty = Difficulty.Easy,
                    Choices = new List<Choice>
                    {
                        new Choice { Text = "2", IsCorrect = true },
                        new Choice { Text = "12" },
                        new Choice { Text = "38" },
                        new Choice { Text = "-2" }
                    }
                },

                new Question
                {
                    Text = "What is 12 ^ 2?",
                    Difficulty = Difficulty.Medium,
                    Choices = new List<Choice>
                    {
                        new Choice { Text = "144", IsCorrect = true },
                        new Choice { Text = "168" },
                        new Choice { Text = "128" },
                        new Choice { Text = "208" }
                    }
                },

                new Question
                {
                    Text = "What is the square root of 196?",
                    Difficulty = Difficulty.Hard,
                    Choices = new List<Choice>
                    {
                        new Choice { Text = "14", IsCorrect = true },
                        new Choice { Text = "13" },
                        new Choice { Text = "12" },
                        new Choice { Text = "15" }
                    }
                }
            };
        }

        public static IList<GameSession> GetSeedGameSessions()
        {
            return new List<GameSession>
                {
                    new GameSession
                    {
                        UserId = 1,
                        StartTime = new DateTime(2018, 01, 22, 14, 33, 00),
                        EndTime = new DateTime(2018, 01, 22, 15, 20, 00),
                        Answers = new List<Answer>
                        {
                            new Answer { ChoiceId = 1 },
                            new Answer { ChoiceId = 2 },
                            new Answer { ChoiceId = 3 },
                            new Answer { ChoiceId = 4 },
                            new Answer { ChoiceId = 5 },
                            new Answer { ChoiceId = 6 },
                            new Answer { ChoiceId = 7 },
                            new Answer { ChoiceId = 8 },
                            new Answer { ChoiceId = 9 }
                        }
                    },

                    new GameSession
                    {
                        UserId = 2,
                        StartTime = new DateTime(2018, 03, 11, 12, 17, 00),
                        EndTime = new DateTime(2018, 03, 11, 12, 45, 32),
                        Answers = new List<Answer>
                        {
                            new Answer { ChoiceId = 7 },
                            new Answer { ChoiceId = 4 },
                            new Answer { ChoiceId = 10 }
                        }
                    }
                };
        }
    }
}
