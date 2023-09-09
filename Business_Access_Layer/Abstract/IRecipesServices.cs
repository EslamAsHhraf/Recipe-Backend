using Data_Access_layer.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business_Access_Layer.Abstract
{
    public interface IRecipesServices
    {
        public IEnumerable<Recipe> GetMyRecipes() ;
    }
}
