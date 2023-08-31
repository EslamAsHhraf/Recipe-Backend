using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Recipe_DataAccess.model
{
    public class UserDto
    {
        [Required(ErrorMessage = "The user name is required")]

        public string Username { get; set; } = string.Empty;
        [Required(ErrorMessage = "The Password is required")]

        public string Password { get; set; } = string.Empty;


    }
}
