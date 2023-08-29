using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Authorization.Models
{
    public class User
    {
        [Column(Order = 0)]
        public int Id { get; set; }
        [Required]

        public string Username { get; set; } 
        [Required]
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
        // should we use mail or not
    }
}
