using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using QuizAppApi.DTOs;
using QuizAppApi.Infrastructure.ServiceResponse;
using QuizAppApi.Models;
using QuizAppApi.Services.Authentication;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace QuizAppApi.Controllers
{
    /// <summary>
    /// AuthController
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthServices _authService;

        public AuthController(IAuthServices authService)
        {
            _authService = authService;
        }
        /// <summary>
        /// Method For Register
        /// </summary>

        [HttpPost("Register")]
        public async Task<ActionResult<ServiceResponse<int>>> Register(UserRegisterDTO request)
        {
            var response = await _authService.Register
                (
                    new User
                    {
                        Name = request.Name,
                        Email=request.Email
                        //Role="Company"
                    },
                    request.Password
                );
            if (!response.Success)
            {
               //new Error(response.Message);
                return BadRequest(response);
            }
            return Ok(response);
        }
        /// <summary>
        /// Method To Login into System.Provides JWT token
        /// </summary>

        [HttpPost("Login")]
        public async Task<ActionResult<ServiceResponse<int>>> Login(UserLoginDTO request)
        {
            var response = await _authService.Login(request.Email, request.Password);
            if (!response.Success)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }
    }
}
