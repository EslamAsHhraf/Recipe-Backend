using Business_Access_Layer.Abstract;
using Data_Access_layer.Model;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace RecipeAPI.Controllers
{
    public class RecipeSearchController : Controller
    {
        private readonly IRecipeIngredientsService _ingredientsService;


        public RecipeSearchController(IRecipeIngredientsService ingredientsService)
        {
            _ingredientsService = ingredientsService;
        }
        [Route("api/recipe/search")]
        [HttpGet]
        public async Task<IEnumerable<Recipe>> SearchRecipeByName(string[] searchTerm)
        {
            var recipes = new List<Recipe>();
            foreach (var Term in searchTerm)
            {
                recipes.AddRange(await _ingredientsService.FilterByIngredients(Term));
            }
            return recipes.Distinct().ToList();
        }
        [Route("api/recipeingredients")]
        [HttpGet]
        public async Task<IEnumerable<RecipeIngredients>> GetMostRepeatedIngredients()
        {
            var ingredients =await _ingredientsService.GetAllIngredients();
            return ingredients;
        }

    }
}