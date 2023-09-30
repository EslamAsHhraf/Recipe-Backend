
using Authorization.Model;
using Business_Access_Layer.Common;
using DomainLayer.Model;
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
        public Task<User> CheckUser(UserDto request);
        public Task<Response> Login(UserDto user);

        public Task<Response> WhoLogin();
        public Task<Response> Register(UserDto user);
        public Task<UserData> GetMe();
        bool CheckPasswordStrength(string password);
        public Task<Response> logout();
        public string CreateToken(User user);
        public void SetJWT(string encrypterToken);
        public Task<Response> changePassword(string oldPassword, string newPassword);
        public Task<Response> SaveImage(IFormFile imageFile);
        public Task<Byte[]> GetImage(string username);
        public void encryptPassword(string password, out byte[] passwordHash, out byte[] passwordKey);
        public bool MatchPasswordHash(string passwordText, byte[] password, byte[] passwordKey);
        public Tuple<string, int, string> GetUserById(int id);
        public Task<Byte[]> GetUserImage(string username);
    }
}

