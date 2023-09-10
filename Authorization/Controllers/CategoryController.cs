using Business_Access_Layer.Abstract;
using Data_Access_layer.Interfaces;
using Data_Access_layer.Model;
using Microsoft.AspNetCore.Mvc;
using RecipeAPI.Common;

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
        public ActionResult<Response> GetAllCategories()
        {
            response.Data = _categoryServices.GetCategories() ;
            response.Status = "success";
            return StatusCode(200, response);
        }
    }
}
