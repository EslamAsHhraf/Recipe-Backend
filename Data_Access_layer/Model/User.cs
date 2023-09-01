using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data_Access_layer.Model
    
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
        public byte[] ImageFile { get; set; }

        // should we use mail or not
    }
}
