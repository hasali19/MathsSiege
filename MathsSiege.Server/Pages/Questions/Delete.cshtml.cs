using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MathsSiege.Server.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MathsSiege.Server.Pages.Questions
{
    public class DeleteModel : PageModel
    {
        private readonly IQuestionRepository questionRepository;

        public int Id { get; private set; }

        public DeleteModel(IQuestionRepository questionRepository)
        {
            this.questionRepository = questionRepository;
        }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var question = await questionRepository.GetQuestionAsync(id);

            if (question == null)
            {
                return NotFound();
            }

            Id = id;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            await questionRepository.DeleteQuestionAsync(id);

            return RedirectToPage("/Questions/Index");
        }
    }
}