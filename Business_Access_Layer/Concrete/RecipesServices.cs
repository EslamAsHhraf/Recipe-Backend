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

namespace Business_Access_Layer.Concrete
{
    public class RecipesServices: IRecipesServices
    {
        private readonly IRecipes _recipesRepository;
        private IAuthService _userService;
        private readonly IWebHostEnvironment _hostEnvironment;

        public RecipesServices(IRecipes recipesRepository, IAuthService userService, IWebHostEnvironment hostEnvironment)
        {
            _recipesRepository = recipesRepository;
            _userService= userService;
            _hostEnvironment = hostEnvironment;
        }
        public IEnumerable<Recipe> GetMyRecipes()
        {
            UserData data=_userService.GetMe();
            if(data==null)
            {
                return null;
            }
            return _recipesRepository.GetMyRecipes(data.Id);
        }
        public async Task<Recipe> SaveImage(IFormFile imageFile, Recipe recipe)
        {
            if (recipe.ImageFile != string.Empty || recipe.ImageFile != "initial-resipe.jpg")
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
    }
}
