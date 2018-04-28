using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MathsSiege.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required]
        [StringLength(20, MinimumLength = 5)]
        public string Username { get; set; }

        [Required]
        [MinLength(8)]
        [DataType(DataType.Password)]
        [RegularExpression("^(?=.*[a-zA-Z])(?=.*[0-9]).+$", ErrorMessage = "Password must contain at least one letter and number.")]
        [Column("PasswordHash")]
        public string Password { get; set; }

        public Role Role { get; set; } = Role.Student;
    }
}
