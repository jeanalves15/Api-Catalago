using System.ComponentModel.DataAnnotations;

namespace Catalogo_Api.DTOs
{
    public class RegisterModel
    {
        [Required(ErrorMessage = "Username is required")]
        public string? Username { get; set; }

        [EmailAddress]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string? Password { get; set; }
    }
}
