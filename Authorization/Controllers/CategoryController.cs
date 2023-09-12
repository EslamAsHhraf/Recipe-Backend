using Business_Access_Layer.Abstract;
using Business_Access_Layer.Common;
using Data_Access_layer.Interfaces;
using Data_Access_layer.Model;
using Microsoft.AspNetCore.Mvc;

namespace RecipeAPI.Controllers
{
    [Route("api/category")]

    public class CategoryController : Controller
    {
        private readonly ICategory _categoryServices;
        private Response response = new Response();

        public CategoryController(ICategory categoryServices)
        {
            _categoryServices = categoryServices;
        }
        [HttpGet]
        public Task<Response> GetAllCategories()
        {
            return  _categoryServices.GetCategories();
        }
    }
}
