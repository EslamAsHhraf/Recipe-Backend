using Business_Access_Layer.Abstract;
using Business_Access_Layer.Common;
using Business_Access_Layer.Concrete;
using Data_Access_layer.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace RecipeAPI.Controllers
{
    [Route("api/Review")]
    [ApiController]
    public class ReviewController : ControllerBase
    {
        private readonly IReviewService _reviewServices;

        public ReviewController(IReviewService reviewServices)
        {
            _reviewServices = reviewServices;
        }
        [HttpGet]
        public ActionResult<Response> GetReviewOfRecipe(int id)
        {
            var data = _reviewServices.GetRecipeReviews(id);
            return StatusCode(Int16.Parse(data.Result.Status), data.Result);
        }
        [HttpPost]
        public async Task<IActionResult> PostReview([FromBody] Review review)
        {
            var data = _reviewServices.CreateReview(review);
            return StatusCode(Int16.Parse(data.Result.Status), data.Result);
        }

    }
}
