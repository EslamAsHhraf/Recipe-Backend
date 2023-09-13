using Business_Access_Layer.Common;
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
        public Task<Response> GetAllIngredients();
        public Task<Response> CreateRecipeIngredient(RecipeIngredients recipeIngredients);
        public Task<Response> DeleteRecipeIngredients(IEnumerable<RecipeIngredients> recipeIngredients);
        public Task<Response> FilterByIngredients(string[] Text);
        public Task<Response> GetRecipeIngredients(Recipe Recipe);
        public Task<Response> GetById(int Id);
        public Task<Response> Delete(RecipeIngredients ingredient);

    }
}
