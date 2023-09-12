using Azure;
using Data_Access_layer.Model;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business_Access_Layer.Abstract
{
    public interface IRecipeIngredientsService
    {
        public Task<IEnumerable<RecipeIngredients>> GetAllIngredients();
        public Task<RecipeIngredients> CreateRecipeIngredient(RecipeIngredients recipeIngredients);
        public void DeleteRecipeIngredients(IEnumerable<RecipeIngredients> recipeIngredients);
        public Task<IEnumerable<Recipe>> FilterByIngredients(string Text);
        public Task<IEnumerable<RecipeIngredients>> GetRecipeIngredients(Recipe Recipe);

    }
}
