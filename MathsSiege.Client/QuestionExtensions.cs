using MathsSiege.Models;
using System;
using System.Collections.Generic;

namespace MathsSiege.Client
{
    public static class QuestionExtensions
    {
        private static Random random = new Random();

        /// <summary>
        /// Gets a list of the possible choices in a random order.
        /// </summary>
        /// <param name="question"></param>
        /// <returns></returns>
        public static IReadOnlyCollection<Choice> GetRandomizedChoices(this Question question)
        {
            List<Choice> choices = new List<Choice>(question.Choices);

            int n = choices.Count;
            while (n > 1)
            {
                n -= 1;
                int k = random.Next(n + 1);
                var choice = choices[k];
                choices[k] = choices[n];
                choices[n] = choice;
            }

            return choices;
        }

        /// <summary>
        /// Gets the number of points the question is worth.
        /// </summary>
        /// <param name="question"></param>
        /// <returns></returns>
        public static int GetPoints(this Question question)
        {
            return (((int)question.Difficulty) + 1) * 50;
        }
    }
}
