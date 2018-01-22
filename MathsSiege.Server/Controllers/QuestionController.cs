using MathsSiege.Models;
using MathsSiege.Server.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MathsSiege.Server.Controllers
{
    [Route("Api/Questions")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class QuestionController : Controller
    {
        private readonly IQuestionRepository questionRepository;

        public QuestionController(IQuestionRepository questionRepository)
        {
            this.questionRepository = questionRepository;
        }

        [HttpGet]
        public async Task<IEnumerable<Question>> Get()
        {
            var questions = await questionRepository.GetQuestionsAndChoicesAsync();
            return questions.Where(question => question.IsActive);
        }
    }
}
