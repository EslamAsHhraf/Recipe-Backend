using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data_Access_layer.Model
{
    public class Shopping : BaseEntity
    {
        public int Id { get; set; }
        public string Title { get; set; }

        public int Quantity { get; set; }
        public bool Purchased { get; set; } = false;
    }
}
