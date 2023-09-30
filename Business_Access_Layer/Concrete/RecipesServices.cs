using Business_Access_Layer.Abstract;
using DomainLayer.Interfaces;
using DomainLayer.Model;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Business_Access_Layer.Common;
using Authorization.Model;
using Firebase.Auth;
using static System.Net.Mime.MediaTypeNames;


namespace Business_Access_Layer.Concrete
{
    public class RecipesServices : IRecipesServices
    {
        private readonly IRepository<Recipe> _recipeRepository;
        private IAuthService _userService;
        private readonly IFileServices _fileServices;
        private readonly IRecipes _recipesRepository;
        private Response response = new Response();

        public RecipesServices(IRepository<Recipe> recipeRepository, IAuthService userService,
             IFileServices fileServices, IRecipes recipesRepository)
        {
            _recipesRepository = recipesRepository;
            _userService = userService;
            _fileServices = fileServices;
            _recipeRepository = recipeRepository;
            _recipeRepository = recipeRepository;
        }

        public async Task<Response> GetMyRecipes()
        {
            UserData data = await _userService.GetMe();
            if (data == null)
            {
                response.Status = "401";
                response.Data = new { Title = "Unauthorized" };
                return response;
            }

            var myRecipes = _recipesRepository.GetMyRecipes(data.Id);

            response.Status = "200";
            response.Data = myRecipes;
            return response;
        }


        public async Task<Response> GetAllRecipes()
        {
            var Recipes = _recipeRepository.GetAll();
            if (Recipes == null)
            {
                response.Status = "404";
                response.Data = new { Title = "No Content" };
                return response;
            }

            response.Status = "200";
            response.Data = Recipes;
            return response;
        }
        public async Task<Response> GetRecipeById(int Id)
        {
            var Recipe = await _recipeRepository.GetById(Id);
            if (Recipe == null)
            {
                response.Status = "404";
                response.Data = new { Title = "No Content" };
                return response;
            }
            response.Status = "200";
            response.Data = Recipe;
            return response;
        }
        public async Task<Response> Update(Recipe recipe)
        {
            var Recipe = await _recipeRepository.Update(recipe);
            if (Recipe == null)
            {
                response.Status = "404";
                response.Data = new { Title = "Failed" };
                return response;
            }
            response.Status = "200";
            response.Data = Recipe;
            return response;
        }
        public async Task<Response> Delete(Recipe recipe)
        {
            _recipeRepository.Delete(recipe);
            response.Status = "200";
            response.Data = new { Title = "Deleted" };
            return response;
        }
        public async Task<Response> SaveImage(IFormFile imageFile, int Id)
        {
            var UserData = await _userService.GetMe();
            if (UserData == null)
            {
                response.Status = "401";
                response.Data = new { Title = "Untheorized User" };
                return response;
            }
            var recipe = await _recipeRepository.GetById(Id);
            if (recipe == null)
            {
                response.Data = new { Title = "recipe not found" };
                response.Status = "404";
                return response;
            }
            if (recipe.CreatedBy != UserData.Id)
            {
                response.Data = new { Title = "Unauthorize user" };
                response.Status = "401";
                return response;
            }


            string image = await SaveImageRecipe(imageFile, recipe);
            if (image == "")
            {
                response.Status = "400";
                response.Data = new { Title = "Error while update image" };
                return response;
            }
            recipe.ImageFile = image;

            await _recipeRepository.Update(recipe);

            response.Status = "200";
            response.Data = new { Title = "Success Update image" };
            return response;
        }



        public async Task<string> SaveImageRecipe(IFormFile imageFile, Recipe recipe)
        {

            string image = await _fileServices.SaveImage(imageFile, "recipe" + recipe.Id);

            return image;
        }
        public async Task<Response> AddRecipe(IFormFile imageFile, Recipe recipe)
        {
            var UserData = await _userService.GetMe();
            if (UserData == null)
            {
                response.Status = "401";
                response.Data = new { Title = "Untheorized User" };
                return response;
            }
            if (recipe.CreatedBy != UserData.Id)
            {
                response.Data = new { Title = "Unauthorize user" };
                response.Status = "401";
                return response;
            }
            if (imageFile != null)
            {
                recipe.ImageFile = "";
                var Recipe = await _recipeRepository.Create(recipe);
                if (Recipe == null)
                {
                    response.Status = "400";
                    response.Data = new { Title = "Error in create" };
                    return response;
                }
                string image = await SaveImageRecipe(imageFile, Recipe);
                if (image == "")
                {
                    response.Status = "400";
                    response.Data = new { Title = "Error while update image" };
                    return response;
                }
                Recipe.ImageFile = image;
                return await Update(Recipe);


            }
            else
            {
                recipe.ImageFile = "https://firebasestorage.googleapis.com/v0/b/imagenet-5a741.appspot.com/o/images%2Finitial-recipe.jpg?alt=media&token=11883e37-3aae-4285-a4be-3dc69ebba6a9&_gl=1*13b4q8d*_ga*MTA4MjAxMzE5My4xNjkyMzQ2NDYx*_ga_CW55HF8NVT*MTY5NjAxMTExNS41LjEuMTY5NjAxNjc3NS4xOS4wLjA.";
                var list = await _recipeRepository.Create(recipe);
                response.Data = list;
            }


            response.Status = "201";
            return response;
        }
    }
}