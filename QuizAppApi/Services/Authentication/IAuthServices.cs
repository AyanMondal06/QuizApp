using QuizAppApi.Infrastructure.ServiceResponse;
using QuizAppApi.Models;
using System.ComponentModel.DataAnnotations;

namespace QuizAppApi.Services.Authentication
{
    public interface IAuthServices
    {
        Task<ServiceResponse<string>> Login(string email, string Password);
        Task<ServiceResponse<int>> Register(User User, string Password);
    }
}
