using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MathsSiege.Models;
using MathsSiege.Server.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MathsSiege.Server.Pages.Questions
{
    public class EditModel : PageModel
    {
        private readonly IQuestionRepository questionRepository;

        [BindProperty]
        public Question Question { get; set; }

        public EditModel(IQuestionRepository questionRepository)
        {
            this.questionRepository = questionRepository;
        }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Question = await questionRepository.GetQuestionAndChoicesAsync(id);

            if (Question == null)
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

            await questionRepository.UpdateQuestionAsync(id, Question);

            ViewData["UpdateSuccess"] = true;

            return Page();
        }
    }
}