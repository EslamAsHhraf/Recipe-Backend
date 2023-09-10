
using Business_Access_Layer.Abstract;
using Data_Access_layer.Interfaces;
using Data_Access_layer.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RecipeAPI.Common;

namespace RecipeAPI.Controllers
{
    [Route("api/recipe")]
    public class RecipesController : Controller
    {
        private readonly IRepository<Recipe> _recipeRepository;
        private readonly IRepository<Category> _categoryRepository;
        private readonly IRecipeIngeradiants<RecipeIngredients> _recipeIngreRepository;
        private readonly IUserRepository _userRepository;
        private readonly IRecipesServices _recipesServices;

        private Response response = new Response();

        public RecipesController(IRepository<Recipe> recipeRepository, IRecipeIngeradiants<RecipeIngredients> recipeIngreRepository, 
            IUserRepository UserRepository, IRepository<Category> categoryRepository, IRecipesServices recipesServices)
        {
            _recipeRepository = recipeRepository;
            _recipeIngreRepository = recipeIngreRepository;
            _userRepository = UserRepository;
            _categoryRepository = categoryRepository;
            _recipesServices = recipesServices;
        }

        [HttpGet]
        public IEnumerable<Recipe> GetAllRecipes()
        {
            return _recipeRepository.GetAll();
        }

        [HttpGet("{id}")]
        public async Task<Tuple<Recipe, IEnumerable<RecipeIngredients>, Tuple<string,int>, Category>> GetRecipeById(int id)
        {
           var recipe =  await _recipeRepository.GetById(id);
           var ingredients = await _recipeIngreRepository.GetRecipeIngredients(recipe);
           var Createdby =  _userRepository.GetUserById(recipe.CreatedBy);
           var Category = await _categoryRepository.GetById(recipe.Category);
           return Tuple.Create(recipe, ingredients, Createdby, Category);
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

        [HttpGet("getMyRecipes"), Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public ActionResult<Response> GetMyRecipes()
        {
            var data=_recipesServices.GetMyRecipes();
           
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
            if (imageFile == null)
            {
                Task<Recipe> result = _recipesServices.SaveImage(imageFile, recipe);
                Recipe recipeResult = await result;
                var list = _recipeRepository.Create(recipeResult);

                response.Data = new { Data = list };
            }
            else
            {
                recipe.ImageFile = "initial-resipe.jpg";
                response.Data = new { Data = recipe };
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


      

