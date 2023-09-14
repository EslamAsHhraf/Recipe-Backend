using Business_Access_Layer.Common;
using Business_Access_Layer.Abstract;
using Data_Access_layer.Interfaces;
using Data_Access_layer.Model;
using Microsoft.AspNetCore.Mvc;

namespace RecipeAPI.Controllers
{
    [Route("api/category")]

    public class CategoryController : Controller
    {
        private readonly ICategory _categoryServices;

        public CategoryController(ICategory categoryServices)
        {
            _categoryServices = categoryServices;
        }
        [HttpGet]
        public async Task<ActionResult<Response>> GetAllCategories()
        {
            var response= await  _categoryServices.GetCategories();
            return StatusCode(Int16.Parse(response.Status), response);

        }
    }
}
