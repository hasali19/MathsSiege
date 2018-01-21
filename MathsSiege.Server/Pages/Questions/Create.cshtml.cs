using MathsSiege.Models;
using MathsSiege.Server.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;

namespace MathsSiege.Server.Pages.Questions
{
    public class CreateModel : PageModel
    {
        private readonly IQuestionRepository questionRepository;

        [BindProperty]
        public Question Question { get; set; }

        public CreateModel(IQuestionRepository questionRepository)
        {
            this.questionRepository = questionRepository;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            await questionRepository.AddQuestionAsync(Question);

            return RedirectToPage("/Questions/Edit", new { Question.Id });
        }
    }
}