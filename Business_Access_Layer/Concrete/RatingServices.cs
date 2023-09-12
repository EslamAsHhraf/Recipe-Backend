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
    public class RatingServices : IRatingService
    {
        private readonly IRepository<Rating> _ratingRepository;
        private Response response = new Response();
        public RatingServices(IRepository<Rating> ratingRepository)
        {
            _ratingRepository = ratingRepository;

        }
        public async Task<Response> GetRecipeRating(int id)
        {
            var ratings = _ratingRepository.GetAll();
            if (ratings == null)
            {
                response.Status = "404";
                response.Data = new { Title = "No ratings available" };
                return response;
            }
            var RecipeRatings = ratings.Where(recipe => recipe.RecipeId == id).ToList();
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
        public async Task<Response> CreateRating(Rating rating)
        {
            if(rating.Rate>5||rating.Rate<0)
            {
                response.Status = "400";
                response.Data = new { Title = "Invalid data value of rating must be from 0 to 5" };
                return response;
            }
            var createdrating = await _ratingRepository.Create(rating);
            response.Status = "200";
            response.Data = createdrating;
            return response;
        }
    }
}
