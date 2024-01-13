using System.ComponentModel.DataAnnotations;

namespace API.DTOs
{
    public class RegisterDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        [RegularExpression("(?=.*\\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[^a-zA-Z0-9]).{4,}$", ErrorMessage = "Password must be complex")]
        public string Password { get; set; }

        [Required]
        public string DisplayName { get; set; }
        public string Username { get; set; }
    }
}