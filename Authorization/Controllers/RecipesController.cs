using Data_Access_layer.Interfaces;
using Data_Access_layer.Model;
using Microsoft.AspNetCore.Mvc;

namespace RecipeAPI.Controllers
{
    [Route("api/recipe")]
    public class RecipesController : Controller
    {
        private readonly IRepository<Recipe> _recipeRepository;
        private readonly IRepository<Category> _categoryRepository;
        private readonly IRecipeIngeradiants<RecipeIngredients> _recipeIngreRepository;
        private readonly IUserRepository _userRepository;

        private Response response = new Response();

        public RecipesController(IRepository<Recipe> recipeRepository
            , IRecipeIngeradiants<RecipeIngredients> recipeIngreRepository
            , IUserRepository UserRepository
            , IRepository<Category> categoryRepository)
        {
            _recipeRepository = recipeRepository;
        }

        [HttpGet]
        public IEnumerable<Recipe> GetAllRecipes()
        {
            return _recipeRepository.GetAll();
        }

        [HttpGet("{id}")]
        public async Task<Tuple<Recipe, IEnumerable<RecipeIngredients>,string,Category>> GetRecipeById(int id)
        {
            return await _recipeRepository.GetById(id);
        }

        [HttpPost]
        public async Task<IActionResult> PostRecipe([FromBody] Recipe recipe)
        {
            await _recipeRepository.Create(recipe);

            return StatusCode(201);
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
