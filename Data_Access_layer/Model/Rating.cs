using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data_Access_layer.Model
{
    public class Rating : BaseEntity
    {
        public int Id { get; set; }
        public int AuthorId { get; set; }
        [ForeignKey("User.UserId")]
        public int RecipeId { get; set; }
        [ForeignKey("Recipe.RecipeId")]
        public int Rate { get; set; }
        
    }
}
