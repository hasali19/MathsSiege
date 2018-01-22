using MathsSiege.Models;
using MathsSiege.Server.Data;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MathsSiege.Server.Pages.GameSessions
{
    public class IndexModel : PageModel
    {
        private readonly IGameSessionRepository gameSessionRepository;

        public ICollection<GameSession> GameSessions { get; private set; }

        public IndexModel(IGameSessionRepository gameSessionRepository)
        {
            this.gameSessionRepository = gameSessionRepository;
        }

        public async Task OnGetAsync()
        {
            GameSessions = await gameSessionRepository.GetGameSessionsAsync();
        }
    }
}