using Authorization.Repository;
using Business_Access_Layer.Abstract;
using Data_Access_layer.Interfaces;
using Data_Access_layer.Model;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
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
        private readonly IRecipes _recipesRepository;
        private IAuthService _userService;
        private readonly IWebHostEnvironment _hostEnvironment;
        private Response response = new Response();

        public RecipesServices(IRecipes recipesRepository, IAuthService userService
            , IWebHostEnvironment hostEnvironment, IRepository<Recipe> recipeRepository)
        {
            _recipesRepository = recipesRepository;
            _userService = userService;
            _hostEnvironment = hostEnvironment;
            _recipeRepository = recipeRepository;
        }
        public IEnumerable<Recipe> GetMyRecipes()
        {
            UserData data = _userService.GetMe();
            if (data == null)
            {
                return null;
            }
            return _recipesRepository.GetMyRecipes(data.Id);
        }

        public async Task<Recipe> SaveImage(IFormFile imageFile, Recipe recipe)
        {
            if (!(recipe.ImageFile == null || recipe.ImageFile == "initial-resipe.jpg"))
            {
                _userService.DeleteImage(recipe.ImageFile);

            }
            string imageName = new String(Path.GetFileNameWithoutExtension(imageFile.FileName).Take(10).ToArray()).Replace(' ', '-');
            imageName = imageName + DateTime.Now.ToString("yymmssfff") + Path.GetExtension(imageFile.FileName);
            var imagePath = Path.Combine(_hostEnvironment.ContentRootPath, "Images", imageName);
            using (var fileStream = new FileStream(imagePath, FileMode.Create))
            {
                await imageFile.CopyToAsync(fileStream);
            }
            recipe.ImageFile = imageName;

            return recipe;
        }
        public Byte[] GetImage(string imageName)
        {
            var imagePath = Path.Combine(_hostEnvironment.ContentRootPath, "Images", imageName);

            Byte[] b = System.IO.File.ReadAllBytes(imagePath);   // You can use your own method over here.         
            return b;
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
            var Recipe =await _recipeRepository.Create(recipe);
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
