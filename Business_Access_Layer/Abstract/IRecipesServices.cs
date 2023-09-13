﻿using Business_Access_Layer.Common;
using Data_Access_layer.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business_Access_Layer.Abstract
{
    public interface IRecipesServices
    {
        public Task<Response> GetMyRecipes() ;
        public Task<Recipe> SaveImage(IFormFile imageFile, Recipe recipe);

        public Task<Response> GetAllRecipes();
        public Task<Response> GetRecipeById(int Id);
        public Task<Response> Update(Recipe recipe);
        public Task<Response> Delete(Recipe recipe);
        public Task<Response> Create(Recipe recipe);

    }
}
