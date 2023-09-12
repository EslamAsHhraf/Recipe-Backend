using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data_Access_layer.Data;
using Data_Access_layer.Interfaces;
using Data_Access_layer.Model;
using Business_Access_Layer.Abstract;

namespace Business_Access_Layer.Concrete
{
   
        public class RecipeIngredientsService : IRecipeIngredientsService
    {
            private IRepository<Recipe> _recipeRepository;
            private IRepository<RecipeIngredients> _recipeIngreRepository;

            public RecipeIngredientsService(IRepository<Recipe> recipeRepository, IRepository<RecipeIngredients> recipeIngreRepository)
            {
                _recipeRepository = recipeRepository; 
                _recipeIngreRepository  = recipeIngreRepository; 
            }
            public async Task<IEnumerable<RecipeIngredients>> GetAllIngredients()
            {
                  var ingredients = _recipeIngreRepository.GetAll();
                  var mostRepeatedIngredients = ingredients
                  .GroupBy(i => i.Title)
                  .OrderByDescending(g => g.Count()).Select(g => g.First()).Take(10);
                  return mostRepeatedIngredients;
            }
            public async Task<RecipeIngredients> CreateRecipeIngredient(RecipeIngredients recipeIngredients)
            {
                return await _recipeIngreRepository.Create(recipeIngredients); 
            }
            public void DeleteRecipeIngredients(IEnumerable<RecipeIngredients> recipeIngredients)
            {
                foreach (RecipeIngredients recipeIngredient in recipeIngredients)
                {
                    _recipeIngreRepository.Delete(recipeIngredient);
                }
            }
            public async Task<IEnumerable<Recipe>> FilterByIngredients(string Text)
            {
                var allrecipes = new List<Recipe>();
           
                var recipefromrecipe = await _recipeRepository.SearchByName(Text);

                var recipeIngredients = await _recipeIngreRepository.SearchByName(Text);

                // Get all recipes that have the matching recipe ingredients.
                var recipesfromIngredients = recipeIngredients.Select(ri => ri.RecipeId).Distinct().ToList();
                var recipes = _recipeRepository.GetAll();
 
                // Get all recipes that match the ingredients.
                var filteredRecipes = recipes.Where(recipe => recipesfromIngredients.Contains(recipe.Id)).ToList();


                allrecipes.AddRange(recipefromrecipe);
                allrecipes.AddRange(filteredRecipes);
                allrecipes = allrecipes.Distinct().ToList();
            
                return (IEnumerable<Recipe>)allrecipes;
            }

            public async Task<IEnumerable<RecipeIngredients>> GetRecipeIngredients(Recipe Recipe)
            {
                // Get all recipe ingredients for the given recipe.
                var recipesIngredients = _recipeIngreRepository.GetAll();
                var filteredRecipeIngredients = recipesIngredients.Where(recipe => recipe.RecipeId == Recipe.Id).ToList();

                return filteredRecipeIngredients;
            }

        }


}
