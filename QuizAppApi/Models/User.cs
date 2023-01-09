using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using QuizAppApi.Infrastructure.Roles;

namespace QuizAppApi.Models
{
    public class User
    {
        [Key]
        public int UserId { get; set; }

        [Column(TypeName = "nvarchar(50)")]

        [EmailAddress]
        public string Email { get; set; }


        public byte[] PasswordHash { get; set; } 
        public byte[] PasswordSalt { get; set; }
        public string Name { get; set; }= String.Empty;

        public int Score { get; set; }

        public int TimeTaken { get; set; }
        public AppRoles Role { get; set; }= AppRoles.Participant;
    }
}
