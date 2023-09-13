using Business_Access_Layer.Abstract;
using Business_Access_Layer.Common;
using Data_Access_layer.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace RecipeAPI.Controllers
{
    [Route("api/rating")]
    public class RatingController : Controller
    {
        private readonly IRatingService _ratingServices;

        public RatingController(IRatingService ratingServices)
        {
            _ratingServices = ratingServices;
        }
        [HttpGet("{id}")]
        public ActionResult<Response> GetRatingOfRecipe(int id)
        {
            var data = _ratingServices.GetRecipeRating(id);
            return StatusCode(Int16.Parse(data.Result.Status), data.Result);
        }
        [HttpPost]
        public async Task<IActionResult> PostRating([FromBody] Rating rating)
        {
            var data = _ratingServices.CreateRating(rating);
            return StatusCode(Int16.Parse(data.Result.Status), data.Result);
        }
    }
}
