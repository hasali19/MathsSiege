using System.Collections.Generic;
using System.Threading.Tasks;
using MathsSiege.Models;

namespace MathsSiege.Server.Data
{
    public interface IQuestionRepository
    {
        Task AddChoiceAsync(int questionId, Choice choice);
        Task AddQuestionAsync(Question question);
        Task DeleteChoiceAsync(int id);
        Task DeleteQuestionAsync(int id);
        Task<Choice> GetChoiceAsync(int id);
        Task<Question> GetQuestionAndChoicesAsync(int id);
        Task<Question> GetQuestionAsync(int id);
        Task<ICollection<Question>> GetQuestionsAndChoicesAsync();
        Task<ICollection<Question>> GetQuestionsAsync();
        Task UpdateChoiceAsync(int id, Choice updated);
        Task UpdateQuestionAsync(int id, Question updated);
    }
}