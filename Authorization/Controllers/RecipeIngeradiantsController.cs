using Business_Access_Layer.Abstract;
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

    }
}
