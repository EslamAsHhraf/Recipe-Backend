﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data_Access_layer.Data;
using Data_Access_layer.Interfaces;
using Data_Access_layer.Model;
using Business_Access_Layer.Abstract;
using Business_Access_Layer.Common;

namespace Business_Access_Layer.Concrete
{
   
        public class RecipeIngredientsService : IRecipeIngredientsService
    {
            private IRepository<Recipe> _recipeRepository;
            private IRepository<RecipeIngredients> _recipeIngreRepository;
            private Response response = new Response();

            public RecipeIngredientsService(IRepository<Recipe> recipeRepository, IRepository<RecipeIngredients> recipeIngreRepository)
            {
                _recipeRepository = recipeRepository; 
                _recipeIngreRepository  = recipeIngreRepository; 
            }
            public async Task<Response> GetAllIngredients()
            {
                  var ingredients = _recipeIngreRepository.GetAll();
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
                var recipe =_recipeIngreRepository.Create(recipeIngredients);
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
                    _recipeIngreRepository.Delete(recipeIngredient);
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

                    var recipeIngredients = await _recipeIngreRepository.SearchByName(Term);

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
                var recipesIngredients = _recipeIngreRepository.GetAll();
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

        }


}