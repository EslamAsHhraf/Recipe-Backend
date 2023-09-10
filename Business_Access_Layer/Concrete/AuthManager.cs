﻿using Business_Access_Layer.Abstract;
using Data_Access_layer.Model;
using Authorization.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Azure.Core;
using Nest;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Hosting;
using Microsoft.SqlServer.Server;
using Data_Access_layer.Interfaces;

namespace Business_Access_Layer.Concrete
{
    public class AuthManager: IAuthService
    {
        private IUserRepository _userRepository;
        public readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IWebHostEnvironment _hostEnvironment;

        public AuthManager(IUserRepository userRepository, IConfiguration configuration, IHttpContextAccessor httpContextAccessor, IWebHostEnvironment hostEnvironment)
        {
            _userRepository = userRepository;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
            _hostEnvironment = hostEnvironment;

        }
        public User Login(UserDto request, out string token)
        {
            token = "";
            var user = _userRepository.Authenticate(request.Username, request.Password);
            if (user == null)
            {
                return null;
            }
            token=CreateToken(user);
            return user;

        }
        public void Register(UserDto request, out string status, out string title)
        {
            // check uniqueness of user name
            if (_userRepository.UserAlreadyExists(request.Username))
            {
                status = "fail";
                title = "User already exists, please try different user name";
                return;
            }

            if (!CheckPasswordStrength(request.Password))
            {
                status = "fail";
                title = "Password must include uppercase and lowercase and digit and special char and min length 8";
                return;
            }

            // check if error happened will saving
            if (!_userRepository.Register(request.Username, request.Password))
            {
                status = "fail";
                title = "Something went wrong while saving";
                return;
            }
            status = "success";
            title = "User created";

        }

        public UserData GetMe()
        {
            var result = string.Empty;
            if (_httpContextAccessor.HttpContext != null)
            {
                result = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Name);
                if (!_userRepository.UserAlreadyExists(result))
                {
                    return null;

                }

            }
            User user = _userRepository.GetUser(result);
            UserData data = new UserData();
            data.Name = user.Username;
            data.Id = user.Id;
            data.ImageFile = user.ImageFile;
            return data;
        }
        public bool CheckPasswordStrength(string password)
        {
            if (password.Any(char.IsDigit) && password.Any(char.IsLower) && password.Any(char.IsUpper) && password.Length >= 8 &&
                password.IndexOfAny("!@#$%^&*?_~-£().,".ToCharArray()) != -1)
                return true;
            return false;
        }
        public bool logout()
        {
            if (_httpContextAccessor.HttpContext.Request.Cookies["token"] != null)
            {
                _httpContextAccessor.HttpContext.Response.Cookies.Delete("token", new CookieOptions
                {
                    Secure = true,
                    SameSite = SameSiteMode.None
                });
                return true;
            }
            return false;

        }
        public string CreateToken(User user)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.NameIdentifier,user.Id.ToString()),
                new Claim(ClaimTypes.Role, "User")
            };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                _configuration.GetSection("AppSettings:Token").Value!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: creds
                );
            var jwt = new JwtSecurityTokenHandler().WriteToken(token);
            SetJWT(jwt);
            return jwt;
        }
        public void SetJWT(string encrypterToken)
        {

            _httpContextAccessor.HttpContext.Response.Cookies.Append("token", encrypterToken,
                  new CookieOptions
                  {
                      Expires = DateTime.Now.AddDays(5),
                      HttpOnly = false,
                      Secure = true,
                      IsEssential = true,
                      SameSite = SameSiteMode.None
                  });
        }
        public void changePassword(string oldPassword, string newPassword, out string status, out string title,out int code)
        {
            var userData = GetMe();
            if (userData == null)
            {
                status = "fail";
                title = "Token not found";
                code = 401;
                return;
            }
            var username = userData.Name;

            var user = _userRepository.Authenticate(username, oldPassword);
            if (user == null)
            {
                status = "fail";
                title = "Wrong Password";
                code = 400;
                return;
            }
            if (!CheckPasswordStrength(newPassword))
            {
                status = "fail";
                title = "Password must include uppercase and lowercase and digit and special char and min length 8";
                code = 400;
                return;
            }
            if (!_userRepository.changePassword( newPassword, user))
            {
                status = "fail";
                title = "Something went wrong while saving";
                code = 400;
                return;
            }
            status = "success";
            title = "Password change successfully";
            code = 200;
            return;
        }

        public async Task<int> SaveImage(IFormFile imageFile)
        {
            var userData = GetMe();
            if (userData == null)
            {
                return 401;
            }
            var username = userData.Name;

            var user =_userRepository.GetUser(username);
            if(user.ImageFile != string.Empty) {
                DeleteImage(user.ImageFile);

            }
            string imageName = new String(Path.GetFileNameWithoutExtension(imageFile.FileName).Take(10).ToArray()).Replace(' ', '-');
            imageName = imageName + DateTime.Now.ToString("yymmssfff") + Path.GetExtension(imageFile.FileName);
            var imagePath = Path.Combine(_hostEnvironment.ContentRootPath, "Images", imageName);
            using (var fileStream = new FileStream(imagePath, FileMode.Create))
            {
                await imageFile.CopyToAsync(fileStream);
            }
            user.ImageFile = imageName;
            if(!_userRepository.updateUser(user))
            {
                return 400;

            }
            return 201; 
        }

        public void DeleteImage(string imageName)
        {
            var imagePath = Path.Combine(_hostEnvironment.ContentRootPath, "Images", imageName);
            if (System.IO.File.Exists(imagePath))
                System.IO.File.Delete(imagePath);
        }
        public Byte[] GetImage()
        {
            var userData = GetMe();
            if (userData == null)
            {
                return null;
            }
            var username = userData.Name;

            var user = _userRepository.GetUser(username);
            if (user == null)
            {
                return null;
            }
            var imagePath = Path.Combine(_hostEnvironment.ContentRootPath, "Images", user.ImageFile);

            Byte[] b = System.IO.File.ReadAllBytes(imagePath);   // You can use your own method over here.         
            return b;
        }

    }
}
