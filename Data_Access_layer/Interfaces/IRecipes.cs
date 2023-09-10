using Data_Access_layer.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data_Access_layer.Interfaces
{
    public interface IRecipes
    {
        public IEnumerable<Recipe> GetMyRecipes(int id);
    }
}
