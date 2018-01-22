using System.Collections.Generic;
using System.Threading.Tasks;
using MathsSiege.Models;

namespace MathsSiege.Server.Data
{
    public interface IGameSessionRepository
    {
        Task AddGameSessionAsync(GameSession gameSession);
        Task DeleteGameSessionAsync(int id);
        Task<GameSession> GetGameSessionAsync(int id);
        Task<ICollection<Answer>> GetGameSessionQuestionsAndAnswersAsync(int id);
        Task<ICollection<GameSession>> GetGameSessionsAsync();
    }
}