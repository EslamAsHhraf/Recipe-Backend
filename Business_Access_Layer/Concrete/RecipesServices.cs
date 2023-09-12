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

        public RecipesServices(IRepository<Recipe> recipesRepository, IAuthService userService, IWebHostEnvironment hostEnvironment)
        {
            _recipesRepository = recipesRepository;
            _userService= userService;
            _hostEnvironment = hostEnvironment;
        }
        public async Task<Response> GetMyRecipes()
        {
            UserData data=_userService.GetMe();
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
    }
}
