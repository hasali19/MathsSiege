﻿using MathsSiege.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MathsSiege.Server.Data
{
    public class GameSessionRepository : IGameSessionRepository
    {
        private readonly AppDbContext context;

        public GameSessionRepository(AppDbContext context)
        {
            this.context = context;
        }

        public async Task<ICollection<GameSession>> GetGameSessionsAsync()
        {
            return await context.GameSessions
                .OrderByDescending(game => game.EndTime)
                .Include(game => game.User)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<GameSession> GetGameSessionAsync(int id)
        {
            return await context.GameSessions
                .Include(game => game.User)
                .FirstOrDefaultAsync(game => game.Id == id);
        }

        public async Task<ICollection<Answer>> GetGameSessionQuestionsAndAnswersAsync(int id)
        {
            return await context.Answers
                .Where(answer => answer.GameSessionId == id)
                .Include(answer => answer.Choice)
                .ThenInclude(choice => choice.Question)
                .ThenInclude(question => question.Choices)
                .ToListAsync();
        }

        public async Task AddGameSessionAsync(GameSession gameSession)
        {
            await context.GameSessions.AddAsync(gameSession);
            await context.SaveChangesAsync();
        }

        public async Task DeleteGameSessionAsync(int id)
        {
            var gameSession = await GetGameSessionAsync(id);

            if (gameSession != null)
            {
                context.GameSessions.Remove(gameSession);
                await context.SaveChangesAsync();
            }
        }
    }
}
