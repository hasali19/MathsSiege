using MathsSiege.Models;
using MathsSiege.Server.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace MathsSiege.Server.Controllers
{
    [Route("Api/GameSessions")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class GameSessionController : Controller
    {
        private readonly IUserRepository userRepository;
        private readonly IGameSessionRepository gameSessionRepository;

        public GameSessionController(IUserRepository userRepository, IGameSessionRepository gameSessionRepository)
        {
            this.userRepository = userRepository;
            this.gameSessionRepository = gameSessionRepository;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] GameSession gameSession)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await userRepository.GetUserAsync(User.Identity.Name);

            if (user == null)
            {
                return NotFound(new { Message = "An error occurred while retrieving the user." });
            }

            gameSession.User = user;
            await gameSessionRepository.AddGameSessionAsync(gameSession);
            
            return Ok(gameSession);
        }
    }
}
