using Business_Access_Layer.Abstract;
using Business_Access_Layer.Common;
using DomainLayer.Interfaces;
using DomainLayer.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business_Access_Layer.Concrete
{
    public class FavouriteService : IFavouriteService
    {
        private readonly IRepository<Favourite> _favouriteRepository;
        private readonly IAuthService _userService;
        private readonly IRepository<Recipe> _recipeRepository;
        private readonly IFileServices _fileServices;

        private Response response = new Response();
        public FavouriteService(IRepository<Favourite> favouriteRepository, IRepository<Recipe> recipeRepository, IFileServices fileServices)
        {
            _favouriteRepository = favouriteRepository;
            _recipeRepository = recipeRepository;
            _fileServices = fileServices;
        }
        public async Task<Response> GetMyFavourites(int id)
        {
            var favourites = _favouriteRepository.GetAll();

            var MyFavourite = favourites.Where(user => user.AuthorId == id).ToList();

            response.Status = "200";
            response.Data = MyFavourite;
            return response;
        }
        public async Task<Response> CreateFavourite(Favourite favourite)
        {
            var createdfavourite = await _favouriteRepository.Create(favourite);
            if (createdfavourite == null)
            {
                response.Status = "404";
                response.Data = new { Title = "Not Created" };
                return response;
            }
            response.Status = "200";
            response.Data = createdfavourite;
            return response;
        }
        public async Task<Response> DeleteFavourite(int favouriteid)
        {
            var favourite = await _favouriteRepository.GetById(favouriteid);
            if (favourite == null)
            {
                response.Status = "404";
                response.Data = new { Title = "Not Created" };
                return response;
            }
            _favouriteRepository.Delete(favourite);

            response.Status = "200";
            response.Data = new { Title = "Deleted" };
            return response;
        }
        public async Task<Response> GetRecipesFavourite(int userid)
        {
            var favourites = _favouriteRepository.GetAll();

            var MyFavourite = favourites.Where(user => user.AuthorId == userid).ToList();
            var allRecipes = _recipeRepository.GetAll();
            var Recipes =
              from recipe in allRecipes
              join fav in MyFavourite on recipe.Id equals fav.RecipeId
              select recipe;

            if (Recipes.Any())
            {
                response.Status = "200";
                response.Data = Recipes;
                return response;
                
            }
            response.Status = "404";
            response.Data = new { Title = "Not found" };
            return response;

        }




    }
}
