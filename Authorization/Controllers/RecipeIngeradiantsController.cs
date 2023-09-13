using Business_Access_Layer.Abstract;
using Business_Access_Layer.Common;
using Business_Access_Layer.Concrete;
using Data_Access_layer.Model;
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

    }
}
