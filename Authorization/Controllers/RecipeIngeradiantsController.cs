using Data_Access_layer.Interfaces;
using Data_Access_layer.Model;
using Microsoft.AspNetCore.Mvc;

namespace RecipeAPI.Controllers
{
    [Route("api/recipeIngredient")]
    public class RecipeIngeradiantsController : Controller
    {
        private readonly IRecipeIngeradiants<RecipeIngredients> _recipeIngredientsRepository;

        public RecipeIngeradiantsController(IRecipeIngeradiants<RecipeIngredients> recipeIngredientsRepository)
        {
            _recipeIngredientsRepository = recipeIngredientsRepository;
        }

        [HttpPost]
        public async Task<IActionResult> PostRecipe([FromBody] RecipeIngredients recipeIngredient)
        {
            var list = _recipeIngredientsRepository.Create(recipeIngredient);
            return StatusCode(201);

        }

    }
}
