using Business_Access_Layer.Abstract;
using Business_Access_Layer.Common;
using Data_Access_layer.Interfaces;
using Data_Access_layer.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace RecipeAPI.Controllers
{
    [Route("api/recipe")]
    public class RecipesController : Controller
    {
        private readonly IRecipeIngredientsService _RecipeIngredientsServices;
        private readonly ICategory _categoryServices;
        private readonly IRecipesServices _recipesServices;
        private readonly IFileServices _fileServices;

        private readonly IAuthService _userService;
        private Response response = new Response();

        public RecipesController( IRecipesServices recipesServices, IAuthService userService, IFileServices fileServices,
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
            var data= _recipesServices.GetAllRecipes();
            return StatusCode(Int16.Parse(data.Result.Status), data.Result);
        }

        [HttpGet("{id}")]
        public ActionResult<Response> GetRecipeById(int id)
        {
            var recipeResponse = _recipesServices.GetRecipeById(id).Result;
            if (recipeResponse.Status == "204")
            {
                return recipeResponse;
            }
            Recipe recipe =(Recipe) _recipesServices.GetRecipeById(id).Result.Data;

            IEnumerable<RecipeIngredients> ingredients = (IEnumerable<RecipeIngredients>) _RecipeIngredientsServices.GetRecipeIngredients(recipe).Result.Data;
            var Createdby = _userService.GetUserById(recipe.CreatedBy);
            Category Category = (Category) _categoryServices.GetCategoryById(recipe.Category).Result.Data;
            Byte[] imageUser = _fileServices.GetImage(recipe.ImageFile);
            var data = Tuple.Create(recipe, ingredients, Createdby, Category, imageUser);
            if (data == null)
            {
                response.Status = "204";
                response.Data = new { Title = "Not Found" };
                return response;
            }
            response.Status = "200";
            response.Data = data;
            return response;            
        }
     
      

        [HttpPut("{id}")]
        public ActionResult<Response> PutRecipe(int id, [FromBody] Recipe recipe)
        {
            var recipeResponse = _recipesServices.GetRecipeById(id).Result;
            if (recipeResponse.Status == "204")
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
        public ActionResult<Response> DeleteRecipe(int id)
        {
            Recipe recipe = (Recipe)_recipesServices.GetRecipeById(id).Result.Data;

            IEnumerable<RecipeIngredients> ingredients = (IEnumerable<RecipeIngredients>)_RecipeIngredientsServices.GetRecipeIngredients(recipe).Result.Data;
            if (recipe == null)
            {
                return NotFound();
            }
            if(ingredients != null)
            {
                var DeleteResponse = _RecipeIngredientsServices.DeleteRecipeIngredients((IEnumerable<RecipeIngredients>)ingredients);
            }

            var DeleteRecipeResponse = _recipesServices.Delete(recipe);

            return StatusCode(Int16.Parse(DeleteRecipeResponse.Result.Status), DeleteRecipeResponse.Result);
            ;
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
                var list = await _recipesServices.Create(recipeResult);

                response.Data = new { Data = list };
            }
            else
            {
                recipe.ImageFile = "initial-resipe.jpg";
                var list = await _recipesServices.Create(recipe);
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

            return StatusCode(Int16.Parse(data.Result.Status), data.Result);

        }




    }
}


      

