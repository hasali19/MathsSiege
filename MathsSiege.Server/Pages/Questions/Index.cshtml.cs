using MathsSiege.Models;
using MathsSiege.Server.Data;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Linq;
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

        public async Task OnGetAsync(bool inActive)
        {
            Questions = await questionRepository.GetQuestionsAsync();

            if (!inActive)
            {
                Questions = Questions.Where(question => question.IsActive).ToList();
            }

            ViewData["InActive"] = inActive;
        }
    }
}