using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data_Access_layer.Model
{
    public class Shopping : BaseEntity
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int CreatedBy { get; set; }
        [ForeignKey("User.UserId")]
        public int QuantityShopping { get; set; }
        public int QuantityPurchased { get; set; } = 0;
    }
}
