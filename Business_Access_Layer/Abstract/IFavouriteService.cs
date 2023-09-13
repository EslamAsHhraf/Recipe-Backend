using Business_Access_Layer.Common;
using Data_Access_layer.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business_Access_Layer.Abstract
{
    public interface IFavouriteService
    {
        public Task<Response> GetMyFavourites(int id);
        public Task<Response> CreateFavourite(Favourite favourite);
        public Task<Response> DeleteFavourite(Favourite favouriteid);

    }
}
