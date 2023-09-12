using Business_Access_Layer.Abstract;
using Business_Access_Layer.Common;
using Data_Access_layer.Interfaces;
using Data_Access_layer.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business_Access_Layer.Concrete
{
    public class ReviewService : IReviewService
    {
        private readonly IRepository<Review> _reviewRepository;
        private Response response = new Response();
        public ReviewService(IRepository<Review> reviewRepository)
        {
            _reviewRepository = reviewRepository;

        }
        public async Task<Response> GetRecipeReviews(int id)
        {
            var reviews = _reviewRepository.GetAll();
            if (reviews == null)
            {
                response.Status = "404";
                response.Data = new { Title = "No Reviews available" };
                return response;
            }
            var RecipeRatings = reviews.Where(recipe => recipe.RecipeId == id).ToList();
            if (RecipeRatings == null)
            {
                response.Status = "404";
                response.Data = new { Title = "No ratings available" };
                return response;
            }
            response.Status = "200";
            response.Data = RecipeRatings;
            return response;

        }
        public async Task<Response> CreateReview(Review review)
        {
            
            var createdrating = await _reviewRepository.Create(review);
            if (createdrating == null)
            {
                response.Status = "404";
                response.Data = new { Title = "Not Created" };
                return response;
            }
            response.Status = "200";
            response.Data = createdrating;
            return response;
        }
    }
}
