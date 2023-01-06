using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuizAppApi.DTOs
{
    public class UserRegisterDTO
    {
        [Required]
        [EmailAddress]
        public String Email { get; set; } =String.Empty;

        [RegularExpression(@"^[A-Za-z][A-Za-z ]*[A-Za-z]$", ErrorMessage = "Only Alphabets and space allowed")]
        public string Name { get; set; } = String.Empty;
        [PasswordPropertyText]
        public string Password { get; set; } = String.Empty;
    }
}
