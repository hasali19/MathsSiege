using MathsSiege.Models;
using MathsSiege.Server.Data;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MathsSiege.Server.Pages.Questions
{
    public class IndexModel : PageModel
    {
        private readonly IQuestionRepository questionRepository;

        public ICollection<Question> Questions { get; private set; }

        public IndexModel(IQuestionRepository questionRepository)
        {
            this.questionRepository = questionRepository;
        }

        public async Task OnGetAsync()
        {
            Questions = await questionRepository.GetQuestionsAsync();
        }
    }
}