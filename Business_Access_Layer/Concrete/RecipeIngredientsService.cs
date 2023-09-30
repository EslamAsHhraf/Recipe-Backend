using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainLayer.Interfaces;
using DomainLayer.Model;
using Business_Access_Layer.Abstract;
using Business_Access_Layer.Common;

namespace Business_Access_Layer.Concrete
{

    public class RecipeIngredientsService : IRecipeIngredientsService
    {
        private DomainLayer.Interfaces.IRepository<Recipe> _recipeRepository;
        private DomainLayer.Interfaces.IRepository<RecipeIngredients> _ingregradientRepository;
        private Response response = new Response();
        private readonly IRecipesServices _recipeServices;
        private readonly IFileServices _fileServices;
        public RecipeIngredientsService(DomainLayer.Interfaces.IRepository<Recipe> recipeRepository, DomainLayer.Interfaces.IRepository<RecipeIngredients> ingregradientRepository,
            IRecipesServices recipeServices, IFileServices fileServices)
        {
            _recipeRepository = recipeRepository;
            _ingregradientRepository = ingregradientRepository;
            _recipeServices = recipeServices;
            _fileServices = fileServices;
        }
        public async Task<Response> GetAllIngredients()
        {
            var ingredients = _ingregradientRepository.GetAll();
            var mostRepeatedIngredients = ingredients
            .GroupBy(i => i.Title)
            .OrderByDescending(g => g.Count()).Select(g => g.First()).Take(10);
            if (mostRepeatedIngredients == null)
            {
                response.Status = "204";
                response.Data = new { Title = "No Content" };
                return response;
            }
            response.Status = "200";
            response.Data = mostRepeatedIngredients;
            return response;
        }
        public async Task<Response> CreateRecipeIngredient(RecipeIngredients recipeIngredients)
        {
            var recipe = _ingregradientRepository.Create(recipeIngredients);
            if (recipe == null)
            {
                response.Status = "404";
                response.Data = new { Title = "Not Created" };
                return response;
            }
            response.Status = "200";
            response.Data = recipe;
            return response;
        }
        public async Task<Response> DeleteRecipeIngredients(IEnumerable<RecipeIngredients> recipeIngredients)
        {
            foreach (RecipeIngredients recipeIngredient in recipeIngredients)
            {
                _ingregradientRepository.Delete(recipeIngredient);
            }
            response.Status = "200";
            response.Data = new { Title = "Deleted" };
            return response;
        }
        public async Task<Response> FilterByIngredients(string[] Text)
        {
            var allrecipes = new List<Recipe>();
            foreach (var Term in Text)
            {
                var recipefromrecipe = await _recipeRepository.SearchByName(Term);

                var recipeIngredients = await _ingregradientRepository.SearchByName(Term);

                // Get all recipes that have the matching recipe ingredients.
                var recipesfromIngredients = recipeIngredients.Select(ri => ri.RecipeId).Distinct().ToList();
                var recipes = _recipeRepository.GetAll();

                // Get all recipes that match the ingredients.
                var filteredRecipes = recipes.Where(recipe => recipesfromIngredients.Contains(recipe.Id)).ToList();


                allrecipes.AddRange(recipefromrecipe);
                allrecipes.AddRange(filteredRecipes);
            }
            allrecipes = allrecipes.Distinct().ToList();
            if (allrecipes == null)
            {
                response.Status = "204";
                response.Data = new { Title = "Not Content" };
                return response;
            }

            response.Status = "200";
            response.Data = allrecipes;
            return response;
        }

        public async Task<Response> GetRecipeIngredients(Recipe Recipe)
        {
            // Get all recipe ingredients for the given recipe.
            var recipesIngredients = _ingregradientRepository.GetAll();
            var filteredRecipeIngredients = recipesIngredients.Where(recipe => recipe.RecipeId == Recipe.Id).ToList();

            if (filteredRecipeIngredients == null)
            {
                response.Status = "204";
                response.Data = new { Title = "Not Content" };
                return response;
            }
            response.Status = "200";
            response.Data = filteredRecipeIngredients;
            return response;
        }
        public async Task<Response> GetById(int Id)
        {
            var ingredient = await _ingregradientRepository.GetById(Id);
            if (ingredient == null)
            {
                response.Status = "404";
                response.Data = new { Title = "Not Found" };
                return response;
            }
            response.Status = "200";
            response.Data = ingredient;
            return response;
        }
        public async Task<Response> Delete(RecipeIngredients ingredient)
        {
            _ingregradientRepository.Delete(ingredient);
            response.Status = "200";
            response.Data = new { Title = "Deleted" };
            return response;
        }
        public async Task<Response> CreateList(List<RecipeIngredients> _object)
        {
            bool check = await _ingregradientRepository.CreateList(_object);
            if (!check)
            {
                response.Status = "400";
                response.Data = new { Title = "Error while create" };
                return response;
            }
            response.Status = "200";
            response.Data = new { Title = "Created Successfully " };
            return response;
        }
        public async Task<Response> DeleteList(int recipeId)
        {
            var recipe = await _recipeServices.GetRecipeById(recipeId);
            if (recipe.Status == "404")
            {
                response.Status = "404";
                response.Data = new { Title = "can't find recipe" };
                return response;
            }
            IEnumerable<RecipeIngredients> ingredients = (IEnumerable<RecipeIngredients>)GetRecipeIngredients((Recipe)recipe.Data).Result.Data;

            bool check = await _ingregradientRepository.DeleteList(ingredients);
            if (!check)
            {
                response.Status = "400";
                response.Data = new { Title = "Error while delete" };
                return response;
            }
            response.Status = "200";
            response.Data = new { Title = "Delete Successfully " };
            return response;

        }

    }


}
