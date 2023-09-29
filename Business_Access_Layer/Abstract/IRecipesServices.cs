using Business_Access_Layer.Common;
using DomainLayer.Model;
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
        public Task<Response> GetMyRecipes();
        public Byte[] GetImage(string imageName);
        public Task<Response> GetAllRecipes();
        public Task<Response> GetRecipeById(int Id);
        public Task<Response> Update(Recipe recipe);
        public Task<Response> Delete(Recipe recipe);
        public Task<Response> Create(Recipe recipe);
        public Task<Response> SaveImage(IFormFile imageFile, int Id);
        public Task<string> SaveImageRecipe(IFormFile imageFile, Recipe recipe);
        public Task<Response> AddRecipe(IFormFile imageFile, Recipe recipe);
    }
}
