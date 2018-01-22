using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MathsSiege.Models;
using MathsSiege.Server.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MathsSiege.Server.Pages.GameSessions
{
    public class DetailsModel : PageModel
    {
        private readonly IGameSessionRepository gameSessionRepository;

        public GameSession GameSession { get; private set; }
        public ICollection<Answer> Answers { get; private set; }
        public TimeSpan Duration { get; private set; }
        public int Score { get; private set; }
        public int Total { get; private set; }

        public DetailsModel(IGameSessionRepository gameSessionRepository)
        {
            this.gameSessionRepository = gameSessionRepository;
        }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            GameSession = await gameSessionRepository.GetGameSessionAsync(id);

            if (GameSession == null)
            {
                return NotFound();
            }

            Answers = await gameSessionRepository.GetGameSessionQuestionsAndAnswersAsync(id);

            Duration = GameSession.EndTime - GameSession.StartTime;
            Score = Answers.Aggregate(0, (accumulator, answer) =>
                answer.Choice.IsCorrect ? accumulator + (int)answer.Choice.Question.Difficulty + 1 : accumulator);
            Total = Answers.Aggregate(0, (accumulator, answer) => accumulator + (int)answer.Choice.Question.Difficulty + 1);

            return Page();
        }
    }
}