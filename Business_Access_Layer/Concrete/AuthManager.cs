using Authorization.Interfaces;
using Azure.Core;
using Azure;
using Business_Access_Layer.Abstract;
using Data_Access_layer.Model;
using Authorization.Model;
using Business_Access_Layer.Common;
using Nest;
using Response = Business_Access_Layer.Common.Response;
using Microsoft.AspNetCore.Mvc;

namespace Business_Access_Layer.Concrete
{
    public class AuthManager: IAuthService
    {
        private IUserRepository _userRepository;
        private Response response = new Response();


        public AuthManager(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }
        public User Login(UserDto request)
        {
            var user = _userRepository.Authenticate(request.Username, request.Password);
            return user;
           
        }

    }
}
