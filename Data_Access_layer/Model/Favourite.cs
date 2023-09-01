using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data_Access_layer.Model
{
    public class Favourite
    {
        public int Id { get; set; }
        public int AuthorId { get; set; }
        public int RecipeId { get; set; }
    }
}
