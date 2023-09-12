
using Authorization.Model;
using Azure.Core;
using Business_Access_Layer.Common;
using Data_Access_layer.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business_Access_Layer.Abstract
{
    public interface IAuthService 
    {
        public Task<Response> Login(UserDto user);
        public Task<Response> Register(UserDto user);
        public Task<UserData> GetMe();
        bool CheckPasswordStrength(string password);
        public bool logout();
        public string CreateToken(User user);
        public void SetJWT(string encrypterToken);
        public Task<Response> changePassword(string oldPassword, string newPassword);
        public Task<int> SaveImage(IFormFile imageFile);
        public void DeleteImage(string imageName);
        public Byte[] GetImage();

    }
}

