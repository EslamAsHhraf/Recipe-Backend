﻿using Business_Access_Layer.Abstract;
using Business_Access_Layer.Common;
using Business_Access_Layer.Concrete;
using DomainLayer.Model;
using Microsoft.AspNetCore.Mvc;

namespace RecipeAPI.Controllers
{
    [Route("api/recipeIngredient")]
    public class RecipeIngeradiantsController : Controller
    {
        private readonly IRecipeIngredientsService _ingredientsService;

        public RecipeIngeradiantsController(IRecipeIngredientsService ingredientsService)
        {
            _ingredientsService = ingredientsService;
        }

        [HttpPost]
        public async Task<IActionResult> PostRecipe([FromBody] RecipeIngredients recipeIngredient)
        {
            var list = _ingredientsService.CreateRecipeIngredient(recipeIngredient);
            return StatusCode(201);

        }

        [HttpDelete("{id}")]

        public ActionResult<Response> DeleteRecipeIngredient(int id)
        {
            var ingredient = _ingredientsService.GetById(id).Result;

            if (ingredient.Status == "404")
            {
                return StatusCode(Int16.Parse(ingredient.Status), ingredient);
            }
           

            var DeleteRecipeResponse = _ingredientsService.Delete((RecipeIngredients)ingredient.Data);

            return StatusCode(Int16.Parse(DeleteRecipeResponse.Result.Status), DeleteRecipeResponse.Result);
            ;
        }
        [HttpPost("listIngredients")]

        public async Task<IActionResult> UpdateListIngredients([FromBody]  List<RecipeIngredients> ingredients)
        {
            var data = await _ingredientsService.CreateList(ingredients);

           
            return StatusCode(Int16.Parse(data.Status), data);
        }

        [HttpDelete("recipe/{recipeId}")]

        public async Task<IActionResult> DeleteListIngredients(int recipeId)
        {
            var data = await _ingredientsService.DeleteList(recipeId);


            return StatusCode(Int16.Parse(data.Status), data);
        }
    }
}
