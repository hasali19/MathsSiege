using MathsSiege.Models;
using MathsSiege.Server.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;

namespace MathsSiege.Server.Pages.Questions.Choices
{
    public class CreateModel : PageModel
    {
        private readonly IQuestionRepository questionRepository;

        [BindProperty]
        public Choice Choice { get; set; }

        public CreateModel(IQuestionRepository questionRepository)
        {
            this.questionRepository = questionRepository;
        }

        public async Task<IActionResult> OnPostAsync(int questionId)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            await questionRepository.AddChoiceAsync(questionId, Choice);

            return RedirectToPage("/Questions/Edit", new { Id = questionId });
        }
    }
}