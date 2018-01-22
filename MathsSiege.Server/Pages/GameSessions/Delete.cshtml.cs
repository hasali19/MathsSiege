using MathsSiege.Server.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;

namespace MathsSiege.Server.Pages.GameSessions
{
    public class DeleteModel : PageModel
    {
        private readonly IGameSessionRepository gameSessionRepository;

        public DeleteModel(IGameSessionRepository gameSessionRepository)
        {
            this.gameSessionRepository = gameSessionRepository;
        }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var gameSession = await gameSessionRepository.GetGameSessionAsync(id);

            if (gameSession == null)
            {
                return NotFound();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            await gameSessionRepository.DeleteGameSessionAsync(id);

            return RedirectToPage("/GameSessions/Index");
        }
    }
}