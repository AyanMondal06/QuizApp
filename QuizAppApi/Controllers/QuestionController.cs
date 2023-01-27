using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuizAppApi.Infrastructure.DBContext;
using QuizAppApi.Infrastructure.Error;
using QuizAppApi.Models;
using System.Reflection.Metadata.Ecma335;

namespace QuizAppApi.Controllers
{
    /// <summary>
    /// Question Controller
    /// </summary>
    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class QuestionController : ControllerBase
    {
        private DbInfo _dbContext;

        public QuestionController(DbInfo dbContext)
        {
            _dbContext = dbContext;
        }


        /// <summary>
        /// Returns all Questions
        /// </summary>
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Questions>>> GetAllQuestion()
        {
            var result = await _dbContext.QuestionsTable.ToListAsync();
            return Ok(result);

        }

        /// <summary>
        /// Returns Question by id
        /// </summary>
        [Authorize(Roles = "Admin,Participant")]
        [HttpGet("{id}")]
        public async Task<ActionResult<Questions>> GetQuestion(int id)
        {
            var question = await _dbContext.QuestionsTable.FindAsync(id);
            try
            {
                if (question == null)
                {
                    new Error("Question "+id+" not present");
                    return NotFound("Question Id Wrong");
                }
            }
            catch(Exception ex)
            {
                new Error(ex);
                return BadRequest();
            }

            return question;
        }

        /// <summary>
        /// Edit Questions
        /// </summary>
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutQuestion(int id, Questions question)
        {
            if (id != question.QuestionId)
            {
                return BadRequest();
            }

            _dbContext.Entry(question).State = EntityState.Modified;

            try
            {
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                new Error(ex);
                
            }

            return Ok("saved changes");
        }

        /// <summary>
        /// add Question
        /// </summary>
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [Route("AddQuestion")]
        public async Task<ActionResult<string>> CreateQuestions(Questions addQuestion)
        {   
            _dbContext.Add(addQuestion);
            await _dbContext.SaveChangesAsync();
            return Ok("Added");
        }


        /// <summary>
        /// Delete Question
        /// </summary>
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteQuestion(int id)
        {
            var question = await _dbContext.QuestionsTable.FindAsync(id);
            if (question == null)
            {
                new Error(id+"not found");
                return NotFound("Id not found");
            }

            _dbContext.QuestionsTable.Remove(question);
            await _dbContext.SaveChangesAsync();

            return Ok("Deleted");
        }

        /// <summary>
        /// Method to get 5 random question
        /// </summary>
        [HttpGet("Fetch5Que")]
        public async Task<ActionResult<IEnumerable<Questions>>> Get5Question()
        {
            var random5Qns = await (_dbContext.QuestionsTable
                 .Select(x => new
                 {
                     QuestionId = x.QuestionId,
                     QuestionInWords = x.QuestionInWords,
                     ImageName = x.ImageName,
                     Options = new string[] { x.Option1, x.Option2, x.Option3, x.Option4 },
                 })
                 .OrderBy(y => Guid.NewGuid())
                 .Take(5)
                 ).ToListAsync();

            return Ok(random5Qns);
        }

        /// <summary>
        /// Returns 5 question with answer when id is given
        /// </summary>
        [Authorize(Roles = "Admin")]
        [HttpPost("getAnswers")]
        public async Task<ActionResult<Questions>> RetrieveAnswers(int[] qnIds)
        {
            var answers = await (_dbContext.QuestionsTable
                .Where(x => qnIds.Contains(x.QuestionId))
                .Select(y => new
                {
                    QuestionId = y.QuestionId,
                    QuestionInWords = y.QuestionInWords,
                    ImageName = y.ImageName,
                    Options = new string[] { y.Option1, y.Option2, y.Option3, y.Option4 },
                    CorrectAns = y.Answer
                })).ToListAsync();
            return Ok(answers);
        }

        private bool QuestionExists(int id)
        {
            return _dbContext.QuestionsTable.Any(e => e.QuestionId == id);
        }
    }
}
