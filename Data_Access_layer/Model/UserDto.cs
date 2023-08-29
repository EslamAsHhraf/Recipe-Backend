using System.ComponentModel.DataAnnotations;

namespace Authorization.Model

{
    public class UserDto
    {
        [Required(ErrorMessage = "The user name is required")]

        public string Username { get; set; } = string.Empty;
        [Required(ErrorMessage = "The Password is required")]

        public string Password { get; set; } = string.Empty;


    }
}
