using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLayer.Model
{
    public class Category : BaseEntity
    {
        public int Id { get; set; }
        public string Title { get; set; }

    }
}
