using Business_Access_Layer.Abstract;
using Data_Access_layer.Interfaces;
using Data_Access_layer.Model;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Business_Access_Layer.Common;
using Microsoft.AspNetCore.Mvc;

namespace Business_Access_Layer.Concrete
{
    public class RecipesServices: IRecipesServices
    {
        private readonly IRepository<Recipe> _recipesRepository;
        private IAuthService _userService;
        private readonly IWebHostEnvironment _hostEnvironment;
        private Response response = new Response();
        private readonly IFileServices _fileServices;


        public RecipesServices(IRepository<Recipe> recipesRepository, IAuthService userService, 
             IFileServices fileServices)
        {
            _recipesRepository = recipesRepository;
            _userService= userService;
            _fileServices = fileServices;

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
            var myRecipe = await _recipesRepository.GetMyCreated(data.Id);
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
    }
}
