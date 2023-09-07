using Data_Access_layer.Interfaces;
using Data_Access_layer.Model;
using Microsoft.AspNetCore.Mvc;
using RecipeAPI.Common;

namespace RecipeAPI.Controllers
{
    [Route("api/recipe")]
    public class RecipesController : Controller
    {
        private readonly IRepository<Recipe> _recipeRepository;
        private Response response = new Response();

        public RecipesController(IRepository<Recipe> recipeRepository)
        {
            _recipeRepository = recipeRepository;
        }

        [HttpGet]
        public IEnumerable<Recipe> GetAllRecipes()
        {
            return _recipeRepository.GetAll();
        }

        [HttpGet("{id}")]
        public async Task<Recipe> GetRecipeById(int id)
        {
            return await _recipeRepository.GetById(id);
        }

        [HttpPost]
        public async Task<IActionResult> PostRecipe([FromBody] Recipe recipe)
        {
            var list = _recipeRepository.Create(recipe);
            response.Data = new { Data = list };
            response.Status = "success";
            return StatusCode(201, response);

        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutRecipe(int id, [FromBody] Recipe recipe)
        {
            var existingRecipe =await _recipeRepository.GetById(id);

            if (existingRecipe == null)
            {
                return NotFound();
            }

            existingRecipe.Title = recipe.Title;
            existingRecipe.Description = recipe.Description;
            existingRecipe.Steps = recipe.Steps;
            existingRecipe.Category = recipe.Category;
            existingRecipe.CreatedBy = recipe.CreatedBy;
            existingRecipe.TotalRating = recipe.TotalRating;
            existingRecipe.ImageFile = recipe.ImageFile;

            _recipeRepository.Update(existingRecipe);

            return StatusCode(201);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRecipe(int id)
        {
            var recipe =await _recipeRepository.GetById(id);

            if (recipe == null)
            {
                return NotFound();
            }

            _recipeRepository.Delete(recipe);

            return StatusCode(201);
        }


    }
}
