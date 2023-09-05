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
            return await _recipeRepository.SearchByName(searchTerm);
        }

        [HttpPost]
        public async Task<IEnumerable<Recipe>> FilterByIngredients(List<int> ingredientIds)
        {
            return await _recipeRepository.FilterByIngredients(ingredientIds);
        }

    }
}
