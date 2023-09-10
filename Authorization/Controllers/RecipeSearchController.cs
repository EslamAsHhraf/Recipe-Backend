using Data_Access_layer.Interfaces;
using Data_Access_layer.Model;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace RecipeAPI.Controllers
{
    
    public class RecipeSearchController : Controller
    {
        private readonly IRecipeIngeradiants<Recipe> _recipeRepository;
        private readonly IRepository<RecipeIngredients> _ingredientsRepository;

        public RecipeSearchController(IRecipeIngeradiants<Recipe> recipeRepository, IRepository<RecipeIngredients> ingredientsRepository)
        {
            _recipeRepository = recipeRepository;
            _ingredientsRepository = ingredientsRepository;
        }
        [Route("api/recipe/search")]
        [HttpGet]
        public async Task<IEnumerable<Recipe>> SearchRecipesByName(string[] searchTerm)
        {
            var recipes = new List<Recipe>();
            foreach (var Term in searchTerm)
            {
                recipes.AddRange(await _recipeRepository.FilterByIngredients(Term));
            }
            return recipes.Distinct().ToList();
        }

        [Route("api/recipeingredients")]
        [HttpGet]
        public IEnumerable<RecipeIngredients> GetMostRepeatedIngredients()
        {
            var ingredients = _ingredientsRepository.GetAll();
            var mostRepeatedIngredients = ingredients
              .GroupBy(i => i.Title)
              .OrderByDescending(g => g.Count()).Select(g => g.First())
              .Take(10);


            return mostRepeatedIngredients;
        }


    }
}
