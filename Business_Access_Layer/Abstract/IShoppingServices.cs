using Business_Access_Layer.Common;
using Data_Access_layer.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business_Access_Layer.Abstract
{
    public interface IShoppingServices
    {
        public Task<Response> GetById(int id);
        public  Task<Response> GetMyShopping();
        public  Task<Response> GetMyPurchased();
        public  Task<Response> AddPurchased(int id, int quantity);
        public Task<Response> AddShopping(List<Shopping> shopping);
        public  Task<Response> Update(Shopping shopping);

        public Task<Response> Delete(Shopping shopping);
        public  Task<Response> Create(Shopping shopping);


    }
}
