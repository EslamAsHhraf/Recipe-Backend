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
    public class FavouriteService : IFavouriteService
    {
        private readonly IRepository<Favourite> _favouriteRepository;
        private readonly IAuthService _userService;

        private Response response = new Response();
        public FavouriteService(IRepository<Favourite> favouriteRepository)
        {
            _favouriteRepository = favouriteRepository;

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
        public async Task<Response> DeleteFavourite(Favourite favouriteid)
        {
               _favouriteRepository.Delete(favouriteid);
        
                response.Status = "200";
                response.Data = new { Title = "Deleted" };
                return response;
        }
       
   
    }
}
