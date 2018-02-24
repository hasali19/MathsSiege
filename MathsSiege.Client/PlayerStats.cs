using MathsSiege.Models;
using System.Collections.Generic;

namespace MathsSiege.Client
{
    public class PlayerStats
    {
        /// <summary>
        /// Gets the player's current points.
        /// </summary>
        public int Points { get; private set; }

        /// <summary>
        /// Gets all answers the player has submitted.
        /// </summary>
        public IReadOnlyCollection<Answer> Answers => this.answers;

        private List<Answer> answers = new List<Answer>();

        /// <summary>
        /// Records a new answer, increasing <see cref="Points"/> if neccessary.
        /// </summary>
        /// <param name="answer"></param>
        public void AddAnswer(Choice answer)
        {
            this.answers.Add(new Answer { Choice = answer });

            // Add points if it was correct.
            if (answer.IsCorrect)
            {
                this.Points += answer.Question.GetPoints();
            }
        }
    }
}
