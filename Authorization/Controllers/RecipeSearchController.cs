using Data_Access_layer.Interfaces;
using Data_Access_layer.Model;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace RecipeAPI.Controllers
{
    [Route("api/recipe/search")]
    public class RecipeSearchController : Controller
    {
        private readonly IRecipeIngeradiants<Recipe> _recipeRepository;

        public RecipeSearchController(IRecipeIngeradiants<Recipe> recipeRepository)
        {
            _recipeRepository = recipeRepository;
        }
        [HttpGet]
        public async Task<IEnumerable<Recipe>> SearchRecipeByName(string searchTerm)
        {
             var recipe = await _recipeRepository.FilterByIngredients(searchTerm);
            return recipe;
        }

    }
}
