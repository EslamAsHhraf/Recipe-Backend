using Business_Access_Layer.Common;
using DomainLayer.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business_Access_Layer.Abstract
{
    public interface IRatingService
    {
        public Task<Response> GetRecipeRating(int id);
        public Task<Response> CreateRating(Rating rating);

    }
}
