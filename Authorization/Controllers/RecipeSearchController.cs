using Business_Access_Layer.Abstract;
using Business_Access_Layer.Common;
using DomainLayer.Model;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using static System.Runtime.InteropServices.JavaScript.JSType;

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
        public ActionResult<Response> SearchRecipeByName(string[] searchTerm)
        {
            var data= _ingredientsService.FilterByIngredients(searchTerm);
          
            return StatusCode(Int16.Parse(data.Result.Status), data.Result);

        }
        [Route("api/recipeingredients")]
        [HttpGet]
        public ActionResult<Response> GetMostRepeatedIngredients()
        {
            var data = _ingredientsService.GetAllIngredients();
            return StatusCode(Int16.Parse(data.Result.Status), data.Result);
        }

    }
}