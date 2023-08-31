using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Recipe_DataAccess.model
{
    public class User : BaseEntity
    {
        public string Username { get; set; }
        [Required]
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
        // should we use mail or not
    }
}
