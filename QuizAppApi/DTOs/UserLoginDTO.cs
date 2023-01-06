using System.ComponentModel.DataAnnotations;

namespace QuizAppApi.DTOs
{
    public class UserLoginDTO
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = String.Empty;
        public string Password { get; set; } = String.Empty;
    }
}
