using Business_Access_Layer.Abstract;
using Data_Access_layer.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RecipeAPI.Common;

namespace RecipeAPI.Controllers
{
    [Route("api/recipe")]
    public class RecipesController : Controller
    {
        private readonly IRecipeIngredientsService _RecipeIngredientsServices;
        private readonly ICategory _categoryServices;
        private readonly IRecipesServices _recipesServices;
        private readonly IAuthService _userService;
        private Response response = new Response();

        public RecipesController(
            ICategory categoryServices
            , IRecipesServices recipesServices, IAuthService userService
            , IRecipeIngredientsService RecipeIngredientsServices)
        {
            _recipesServices = recipesServices;
            _userService = userService;
            _categoryServices = categoryServices;
            _RecipeIngredientsServices = RecipeIngredientsServices;
        }

        [HttpGet]
        public async Task<IEnumerable<Recipe>> GetAllRecipes()
        {
            return await _recipesServices.GetAllRecipes();
        }

        [HttpGet("{id}")]
        public async Task<Tuple<Recipe, IEnumerable<RecipeIngredients>, Tuple<string, int>, Category, Byte[]>> GetRecipeById(int id)
        {
            var recipe = await _recipesServices.GetRecipeById(id);
            var ingredients = await _RecipeIngredientsServices.GetRecipeIngredients(recipe);
            var Createdby = _userService.GetUserById(recipe.CreatedBy);
            var Category = await _categoryServices.GetCategoryById(recipe.Category);
            Byte[] imageUser = _recipesServices.GetImage(recipe.ImageFile);
            return Tuple.Create(recipe, ingredients, Createdby, Category, imageUser);
        }
     
      

        [HttpPut("{id}")]
        public async Task<IActionResult> PutRecipe(int id, [FromBody] Recipe recipe)
        {
            var existingRecipe =await _recipesServices.GetRecipeById(id);

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

            _recipesServices.Update(existingRecipe);

            return StatusCode(201);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRecipe(int id)
        {
            var recipe =await _recipesServices.GetRecipeById(id);
            var ingredients = await _RecipeIngredientsServices.GetRecipeIngredients(recipe);
            if (recipe == null)
            {
                return NotFound();
            }
            if(ingredients != null)
            {
                _RecipeIngredientsServices.DeleteRecipeIngredients(ingredients);
            }

            _recipesServices.Delete(recipe);

            return StatusCode(201);
        }

      
        [HttpPost, Authorize]
        public async Task<IActionResult> PostRecipe(IFormFile imageFile, [FromQuery] Recipe recipe)
        {
            var UserData = _userService.GetMe();
            if (UserData == null)
            {
                response.Data = new { Title = "Token not found" };
                response.Status = "fail";
                return StatusCode(401, response);
            }
            if (recipe.CreatedBy != UserData.Id)
            {
                response.Data = new { Title = "Unauthorize user" };
                response.Status = "fail";
                return StatusCode(401, response);
            }
            if (imageFile != null)
            {
                Task<Recipe> result = _recipesServices.SaveImage(imageFile, recipe);
                Recipe recipeResult = await result;
                var list = _recipesServices.Create(recipeResult);

                response.Data = new { Data = list };
            }
            else
            {
                recipe.ImageFile = "initial-resipe.jpg";
                var list = _recipesServices.Create(recipe);
                response.Data = new { Data = list };
            }


            response.Status = "success";
            return StatusCode(201, response);

        }


        [HttpGet("getMyRecipes"), Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public ActionResult<Response> GetMyRecipes()
        {
            var data = _recipesServices.GetMyRecipes();

            if (data == null)
            {
                response.Status = "fail";
                response.Data = new { Title = "Unauthorized" };
                return StatusCode(401, response);
            }
            response.Status = "success";
            response.Data = data;
            return StatusCode(200, response);

        }




    }
}


      

