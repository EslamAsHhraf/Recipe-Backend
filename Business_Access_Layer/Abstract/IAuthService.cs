using Authorization.Model;
using Azure.Core;
using Data_Access_layer.Model;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business_Access_Layer.Abstract
{
    public interface IAuthService 
    {
        //T Register(User user);
        public User Login(UserDto user);
        //bool UserExists(string username);
        //void CreateAccessToken(User user);
    }
}
