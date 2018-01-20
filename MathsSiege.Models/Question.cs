using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MathsSiege.Models
{
    public class Question
    {
        public int Id { get; set; }

        [Required]
        [StringLength(400)]
        public string Text { get; set; }

        public Difficulty Difficulty { get; set; }

        public ICollection<Choice> Choices { get; set; }
    }
}
