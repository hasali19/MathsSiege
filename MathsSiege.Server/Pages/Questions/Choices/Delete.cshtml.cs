using MathsSiege.Server.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;

namespace MathsSiege.Server.Pages.Questions.Choices
{
    public class DeleteModel : PageModel
    {
        private readonly IQuestionRepository questionRepository;

        public DeleteModel(IQuestionRepository questionRepository)
        {
            this.questionRepository = questionRepository;
        }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var choice = await questionRepository.GetChoiceAsync(id);

            if (choice == null)
            {
                return NotFound();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            var choice = await questionRepository.GetChoiceAsync(id);
            await questionRepository.DeleteChoiceAsync(id);

            return RedirectToPage("/Questions/Edit", new { Id = choice.QuestionId });
        }
    }
}