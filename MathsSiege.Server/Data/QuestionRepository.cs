using MathsSiege.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MathsSiege.Server.Data
{
    public class QuestionRepository : IQuestionRepository
    {
        private AppDbContext context;

        public QuestionRepository(AppDbContext context)
        {
            this.context = context;
        }

        public async Task<ICollection<Question>> GetQuestionsAsync()
        {
            return await context.Questions
                .OrderBy(question => question.Difficulty)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<ICollection<Question>> GetQuestionsAndChoicesAsync()
        {
            return await context.Questions
                .OrderBy(question => question.Difficulty)
                .Include(question => question.Choices)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Question> GetQuestionAsync(int id)
        {
            return await context.Questions.FindAsync(id);
        }

        public async Task<Question> GetQuestionAndChoicesAsync(int id)
        {
            return await context.Questions
                .Include(question => question.Choices)
                .FirstOrDefaultAsync(question => question.Id == id);
        }

        public async Task<Choice> GetChoiceAsync(int id)
        {
            return await context.Choices.FindAsync(id);
        }

        public async Task AddQuestionAsync(Question question)
        {
            await context.Questions.AddAsync(question);
            await context.SaveChangesAsync();
        }

        public async Task AddChoiceAsync(int questionId, Choice choice)
        {
            var question = await GetQuestionAndChoicesAsync(questionId);
            
            if (question != null)
            {
                question.Choices.Add(choice);
                await context.SaveChangesAsync();
            }
        }

        public async Task<Question> UpdateQuestionAsync(int id, Question updated)
        {
            Question question = await GetQuestionAsync(id);

            if (question != null)
            {
                question.Text = updated.Text ?? question.Text;
                question.Difficulty = updated.Difficulty;

                await context.SaveChangesAsync();

                return question;
            }

            return null;
        }

        public async Task<Choice> UpdateChoiceAsync(int id, Choice updated)
        {
            Choice choice = await GetChoiceAsync(id);

            if (choice != null)
            {
                choice.Text = updated.Text ?? choice.Text;
                choice.IsCorrect = updated.IsCorrect;

                await context.SaveChangesAsync();

                return choice;
            }

            return null;
        }

        public async Task DeleteQuestionAsync(int id)
        {
            Question question = await GetQuestionAsync(id);

            if (question != null)
            {
                context.Questions.Remove(question);
                await context.SaveChangesAsync();
            }
        }

        public async Task DeleteChoiceAsync(int id)
        {
            Choice choice = await GetChoiceAsync(id);

            if (choice != null)
            {
                context.Choices.Remove(choice);
                await context.SaveChangesAsync();
            }
        }
    }
}
