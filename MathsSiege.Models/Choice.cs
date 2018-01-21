using System.ComponentModel.DataAnnotations;

namespace MathsSiege.Models
{
    public class Choice
    {
        public int Id { get; set; }
        public int QuestionId { get; set; }

        [Required]
        [StringLength(80)]
        public string Text { get; set; }

        public bool IsCorrect { get; set; }
    }
}