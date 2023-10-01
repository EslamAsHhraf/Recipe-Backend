using Authorization.Model;
using Business_Access_Layer.Abstract;
using Business_Access_Layer.Authorization;
using Business_Access_Layer.Common;
using DomainLayer.Interfaces;
using DomainLayer.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace RecipeAPI.Controllers
{
    [Route("api/recipe")]
    [ApiController]
    public class RecipesController : Controller
    {
        private readonly IRecipeIngredientsService _RecipeIngredientsServices;
        private readonly ICategory _categoryServices;
        private readonly IRecipesServices _recipesServices;
        private readonly IFileServices _fileServices;

        private readonly IAuthService _userService;
        private Response response = new Response();

        public RecipesController(IRecipesServices recipesServices, IAuthService userService, IFileServices fileServices,
            ICategory categoryServices, IRecipeIngredientsService RecipeIngredientsServices)
        {
            _recipesServices = recipesServices;
            _userService = userService;
            _fileServices = fileServices;
            _categoryServices = categoryServices;
            _RecipeIngredientsServices = RecipeIngredientsServices;
        }

        [HttpGet]
        public ActionResult<Response> GetAllRecipes()
        {
            var data = _recipesServices.GetAllRecipes();
            return StatusCode(Int16.Parse(data.Result.Status), data.Result);
        }
        [HttpGet("user/{userid}")]
        public ActionResult<Response> GetUserRecipes(int userid)
        {
            var data = _recipesServices.GetUSerRecipes(userid);
            return StatusCode(Int16.Parse(data.Result.Status), data.Result);
        }

        [HttpGet("{id}")]
        public ActionResult<Response> GetRecipeById(int id)
        {
            var recipeResponse = _recipesServices.GetRecipeById(id).Result;
            if (recipeResponse.Status == "404")
            {
                return StatusCode(Int16.Parse(recipeResponse.Status), recipeResponse);
            }
            Recipe recipe = (Recipe)_recipesServices.GetRecipeById(id).Result.Data;

            IEnumerable<RecipeIngredients> ingredients = (IEnumerable<RecipeIngredients>)_RecipeIngredientsServices.GetRecipeIngredients(recipe).Result.Data;
            var Createdby = _userService.GetUserById(recipe.CreatedBy);
            Category Category = (Category)_categoryServices.GetCategoryById(recipe.Category).Result.Data;
            var data = Tuple.Create(recipe, ingredients, Createdby, Category);
            if (data == null)
            {
                response.Status = "404";
                response.Data = new { Title = "Not Found" };
                return StatusCode(Int16.Parse(response.Status), recipeResponse);
            }
            response.Status = "200";
            response.Data = data;
            return StatusCode(Int16.Parse(response.Status), response);
        }



        [HttpPut("{id}")]
        public ActionResult<Response> PutRecipe(int id, [FromBody] Recipe recipe)
        {
            var recipeResponse = _recipesServices.GetRecipeById(id).Result;
            if (recipeResponse.Status == "404")
            {
                return recipeResponse;
            }
            Recipe existingRecipe = (Recipe)_recipesServices.GetRecipeById(id).Result.Data;

            existingRecipe.Title = recipe.Title;
            existingRecipe.Description = recipe.Description;
            existingRecipe.Steps = recipe.Steps;
            existingRecipe.Category = recipe.Category;
            existingRecipe.CreatedBy = recipe.CreatedBy;
            existingRecipe.TotalRating = recipe.TotalRating;
            existingRecipe.ImageFile = recipe.ImageFile;

            var updatedrecipeResponse = _recipesServices.Update(existingRecipe);

            return StatusCode(Int16.Parse(updatedrecipeResponse.Result.Status), updatedrecipeResponse.Result);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<Response>> DeleteRecipe(int id)
        {
            var recipe = await _recipesServices.GetRecipeById(id);
            if (recipe.Status == "404")
            {
                return StatusCode(Int16.Parse(recipe.Status), recipe);

            }
            IEnumerable<RecipeIngredients> ingredients = (IEnumerable<RecipeIngredients>)_RecipeIngredientsServices.GetRecipeIngredients((Recipe)recipe.Data).Result.Data;
            if (recipe == null)
            {
                return NotFound();
            }
            if (ingredients != null)
            {
                var DeleteResponse = _RecipeIngredientsServices.DeleteRecipeIngredients((IEnumerable<RecipeIngredients>)ingredients);
            }

            var DeleteRecipeResponse = _recipesServices.Delete((Recipe)recipe.Data);

            return StatusCode(Int16.Parse(DeleteRecipeResponse.Result.Status), DeleteRecipeResponse.Result);
            ;
        }


        [HttpPost]
        public ActionResult<Response> PostRecipe(IFormFile imageFile, [FromQuery] Recipe recipe)
        {

            var data = _recipesServices.AddRecipe(imageFile, recipe);

            return StatusCode(Int16.Parse(data.Result.Status), data.Result);

        }


        [HttpGet("getMyRecipes")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public ActionResult<Response> GetMyRecipes()
        {
            var data = _recipesServices.GetMyRecipes();

            return StatusCode(Int16.Parse(data.Result.Status), data.Result);

        }
        [HttpPut("updateImage/{id}")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> UpdateImage(IFormFile ImageFile, int id)
        {

            if (ImageFile == null)
            {
                response.Status = "404";
                response.Data = new { Title = "ImageFile is null" };
                return StatusCode(404, response); ;
            }

            var data = _recipesServices.SaveImage(ImageFile, id);
            return StatusCode(Int16.Parse(data.Result.Status), data.Result);
        }



    }
}




