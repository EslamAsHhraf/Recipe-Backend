using Business_Access_Layer.Abstract;
using Business_Access_Layer.Common;
using Business_Access_Layer.Concrete;
using Data_Access_layer.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace RecipeAPI.Controllers
{
    [Route("api/Favourite")]
    [ApiController]
    public class FavouriteController : ControllerBase
    {
        private readonly IFavouriteService _favouriteServices;

        public FavouriteController(IFavouriteService favouriteServices)
        {
            _favouriteServices = favouriteServices;
        }
        [HttpGet("Favouritesofuser")]
        public ActionResult<Response> GetFavouritesofuser(int id)
        {
            var data = _favouriteServices.GetMyFavourites(id);
            return StatusCode(Int16.Parse(data.Result.Status), data.Result);
        }
        [HttpPost]
        public async Task<IActionResult> PostFavourite([FromBody] Favourite favourite)
        {
            var data = _favouriteServices.CreateFavourite(favourite);
            return StatusCode(Int16.Parse(data.Result.Status), data.Result);
        }
        [HttpDelete]
        public async Task<IActionResult> DeleteFavourite(int id)
        {
            var data = _favouriteServices.DeleteFavourite(id);
            return StatusCode(Int16.Parse(data.Result.Status), data.Result);
        }
        [HttpGet]
        public async Task<IActionResult> GetFavourite(int id)
        {
            var data = _favouriteServices.GetRecipesFavourite(id);
            return StatusCode(Int16.Parse(data.Result.Status), data.Result);
        }
    }
}
