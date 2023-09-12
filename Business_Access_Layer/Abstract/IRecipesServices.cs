using Data_Access_layer.Model;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business_Access_Layer.Abstract
{
    public interface IRecipesServices
    {
        public IEnumerable<Recipe> GetMyRecipes() ;
        public Task<Recipe> SaveImage(IFormFile imageFile, Recipe recipe);
        public Byte[] GetImage(string imageName);
        public Task<IEnumerable<Recipe>> GetAllRecipes();
        public Task<Recipe> GetRecipeById(int Id);
        public Task<Recipe> Update(Recipe recipe);
        public void Delete(Recipe recipe);
        public Task<Recipe> Create(Recipe recipe);

    }
}
