﻿using MathsSiege.Models;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace MathsSiege.Client
{
    public class PlayerStats
    {
        private int points = 200;
        /// <summary>
        /// The player's current points count.
        /// </summary>
        public int Points
        {
            get => points;
            set
            {
                points = MathHelper.Max(value, 0);
            }
        }

        /// <summary>
        /// Gets all answers the player has submitted.
        /// </summary>
        public IReadOnlyCollection<Answer> Answers => answers;

        private List<Answer> answers = new List<Answer>();

        /// <summary>
        /// Records a new answer, increasing <see cref="Points"/> if neccessary.
        /// </summary>
        /// <param name="answer"></param>
        public void AddAnswer(Choice answer)
        {
            answers.Add(new Answer { Choice = answer });

            // Add points if it was correct.
            if (answer.IsCorrect)
            {
                Points += answer.Question.GetPoints();
            }
        }
    }
}
