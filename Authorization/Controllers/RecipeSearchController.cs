using Data_Access_layer.Interfaces;
using Data_Access_layer.Model;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace RecipeAPI.Controllers
{
    [Route("api/recipe/search")]
    public class RecipeSearchController : Controller
    {
        private readonly IRepository<Recipe> _recipeRepository;

        public RecipeSearchController(IRepository<Recipe> recipeRepository)
        {
            _recipeRepository = recipeRepository;
        }
        [HttpGet]
        public async Task<IEnumerable<Recipe>> SearchRecipeByName(string searchTerm)
        {
            var recipes =await  _recipeRepository.SearchByName(searchTerm);

            return recipes;
        }

        [HttpPost]
        public List<Recipe> FilterByIngredients(List<int> ingredientIds)
        {
            return  _recipeRepository.GetRecipesByIds(ingredientIds);
        }

    }
}
