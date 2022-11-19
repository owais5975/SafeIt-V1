using System.ComponentModel.DataAnnotations;

namespace SafeIt.Models
{
    public class UserLoginModel
    {
        [Required(ErrorMessage = "Email is Required")]
        public string Email { get; set; } = String.Empty;

        [Required(ErrorMessage = "Password is Required")]
        public string Password { get; set; } = String.Empty;
    }
}
