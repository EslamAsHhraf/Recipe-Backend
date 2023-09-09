using Data_Access_layer.Data;
using Data_Access_layer.Interfaces;
using Data_Access_layer.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Data_Access_layer.Repository
{
    public class RecipeIngredientsRepository<T> : IRecipeIngeradiants<T> where T : BaseEntity
    {
        private readonly IRepository<Recipe> _recipeRepository;
        private readonly IRepository<RecipeIngredients> _recipeIngreRepository;

        private readonly DataContext _context;
        public RecipeIngredientsRepository(DataContext context, IRepository<Recipe> recipeRepository, IRepository<RecipeIngredients> recipeIngreRepository)
        {
            _context = context;
            _recipeRepository = recipeRepository;
            _recipeIngreRepository = recipeIngreRepository;
        }
        public async Task<RecipeIngredients> Create(RecipeIngredients recipeIngredients)
        {
             _context.Add(recipeIngredients);
             _context.SaveChangesAsync();

            return recipeIngredients;
        }
        public bool Save()
        {
            var saved = _context.SaveChanges();
            return saved > 0 ? true : false;
        }


        public async Task<IEnumerable<Recipe>> FilterByIngredients(string Text)
        {
            var allrecipes = new List<Recipe>();

            var recipefromrecipe = await _recipeRepository.SearchByName(Text);

            var recipeIngredients = await _recipeIngreRepository.SearchByName(Text);

            // Get all recipes that have the matching recipe ingredients.
            var recipesfromIngredients = recipeIngredients.Select(ri => ri.RecipeId).Distinct().ToList();

            // Get all recipes that match the ingredients.
            var filteredRecipes = await _context.Recipes.Where(recipe => recipesfromIngredients.Contains(recipe.Id)).ToListAsync();


            allrecipes.AddRange(recipefromrecipe);
            allrecipes.AddRange(filteredRecipes);
            allrecipes = allrecipes.Distinct().ToList();

            return (IEnumerable<Recipe>)allrecipes;
        }

        public async Task<IEnumerable<RecipeIngredients>> GetRecipeIngredients(Recipe Recipe)
        {
            // Get all recipe ingredients for the given recipe.
            var filteredRecipeIngredients = await _context.RecipeIngredients.Where(recipe => recipe.RecipeId == Recipe.Id).ToListAsync();

            return filteredRecipeIngredients;
        }

    }
}
