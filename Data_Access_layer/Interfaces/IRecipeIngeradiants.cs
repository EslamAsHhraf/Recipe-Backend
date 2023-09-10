
using Data_Access_layer.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data_Access_layer.Interfaces
{
    public interface IRecipeIngeradiants<T> where T : BaseEntity
    {
        Task<RecipeIngredients> Create(RecipeIngredients recipeIngredients);
        bool Save();
        public Task<IEnumerable<Recipe>> FilterByIngredients(string searchTerm);
        public Task<IEnumerable<RecipeIngredients>> GetRecipeIngredients(Recipe Recipe);

    }
}
