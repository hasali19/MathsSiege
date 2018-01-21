using MathsSiege.Models;
using MathsSiege.Server.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;

namespace MathsSiege.Server.Pages.Questions.Choices
{
    public class EditModel : PageModel
    {
        private IQuestionRepository questionRepository;

        [BindProperty]
        public Choice Choice { get; set; }

        public EditModel(IQuestionRepository questionRepository)
        {
            this.questionRepository = questionRepository;
        }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Choice = await questionRepository.GetChoiceAsync(id);

            if (Choice == null)
            {
                return NotFound();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            Choice = await questionRepository.UpdateChoiceAsync(id, Choice);

            ViewData["UpdateSuccess"] = true;

            return Page();
        }
    }
}