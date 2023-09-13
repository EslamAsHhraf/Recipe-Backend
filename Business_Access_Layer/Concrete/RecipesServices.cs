using Business_Access_Layer.Abstract;
using Data_Access_layer.Interfaces;
using Data_Access_layer.Model;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Business_Access_Layer.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Business_Access_Layer.Common;

namespace Business_Access_Layer.Concrete
{
    public class RecipesServices : IRecipesServices
    {
        private readonly IRepository<Recipe> _recipeRepository;
        private IAuthService _userService;
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly IFileServices _fileServices;
        private readonly IRecipes _recipesRepository;
        private Response response = new Response();

        public RecipesServices(IRepository<Recipe> recipeRepository, IAuthService userService, 
             IFileServices fileServices,IRecipes recipesRepository)
        {
            _recipesRepository = recipesRepository;
            _userService= userService;
            _fileServices = fileServices;
            _recipeRepository = recipeRepository;
            _recipeRepository = recipeRepository;
        }
       
        public async Task<Response> GetMyRecipes()
        {
            UserData data= await _userService.GetMe();
            if(data==null)
            {
                response.Status = "401";
                response.Data = new { Title = "Unauthorized" };
                return response;
            }
            var myRecipe = await _recipeRepository.GetMyCreated(data.Id);
            response.Status = "200";
            response.Data = myRecipe;
            return response;
        }

        public async Task<Recipe> SaveImage(IFormFile imageFile, Recipe recipe)
        {
            if (!(recipe.ImageFile == null || recipe.ImageFile == "initial-resipe.jpg"))
            {
                _fileServices.DeleteImage(recipe.ImageFile);

            }

            recipe.ImageFile = await _fileServices.SaveImage(imageFile);

            return recipe;
        }
        public async Task<Response> GetAllRecipes()
        {
            var Recipes = _recipeRepository.GetAll();
            if (Recipes == null)
            {
                response.Status = "204";
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
                response.Status = "204";
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
                response.Status = "204";
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
        public async Task<Response> Create(Recipe recipe)
        {
            var Recipe = await _recipeRepository.Create(recipe);
            if (Recipe == null)
            {
                response.Status = "204";
                response.Data = new { Title = "Not Found" };
                return response;
            }
            response.Status = "200";
            response.Data = Recipe;
            return response;
        }
    }
}
