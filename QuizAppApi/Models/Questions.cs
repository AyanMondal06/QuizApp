using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace QuizAppApi.Models
{
    public class Questions
    {
        [Key]
        public int QuestionId { get; set; }

        [Column(TypeName = "nvarchar(250)")]
        public string QuestionInWords { get; set; }=String.Empty;

        [Column(TypeName = "nvarchar(50)")]
        public string ImageName { get; set; }=String.Empty ;

        [Column(TypeName = "nvarchar(50)")]
        public string Option1 { get; set; }=String.Empty;

        [Column(TypeName = "nvarchar(50)")]
        public string Option2 { get; set; }=String.Empty ;

        [Column(TypeName = "nvarchar(50)")]
        public string Option3 { get; set; } = String.Empty;

        [Column(TypeName = "nvarchar(50)")]
        public string Option4 { get; set; } = String.Empty;
        [Range(0,3,ErrorMessage ="0-3 allowed")]
        public int Answer { get; set; } 
    }
}
