using Authorization.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace Business_Access_Layer
{
    public class UserBAL
    {
        private Data_Access_layer.UserDAL _DAL;
     
        public UserBAL(IConfiguration configuration, IUserRepository userService)
        {
            _DAL = new Data_Access_layer.UserDAL(configuration, userService);
        }
        public ActionResult<string> GetMe()
        {
            return _DAL.GetMe();
            //var data = _DAL.GetMe()
            //here you can filter or Map 
        }


    }
}
