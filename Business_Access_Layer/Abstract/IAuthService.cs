using Authorization.Model;
using Azure.Core;
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

        public User Login(UserDto user, out string token);
        public void Register(UserDto user, out string status, out string title);
        public string GetMyName();
        bool CheckPasswordStrength(string password);
        public bool logout();
        public string CreateToken(User user);
        public void SetJWT(string encrypterToken);
        public void changePassword(string oldPassword, string newPassword, out string status, out string title, out int code);
        public Task<int> SaveImage(IFormFile imageFile);
        public void DeleteImage(string imageName);


    }
}
