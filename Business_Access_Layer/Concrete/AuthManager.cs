using Business_Access_Layer.Abstract;
using Data_Access_layer.Model;
using Authorization.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Hosting;
using System.Security.Cryptography;
using Data_Access_layer.Interfaces;
using Business_Access_Layer.Common;
using Azure.Core;

namespace Business_Access_Layer.Concrete
{
    public class AuthManager: IAuthService
    {
        private readonly IUserRepository<User> _userRepository;
        public readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IWebHostEnvironment _hostEnvironment;
        private Response response = new Response();

        public AuthManager(IUserRepository<User> userRepository, IConfiguration configuration, IHttpContextAccessor httpContextAccessor, IWebHostEnvironment hostEnvironment)
        {
            _userRepository = userRepository;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
            _hostEnvironment = hostEnvironment;

        }
        public async Task<User> ChekcUser(UserDto request)
        {
            User user = await _userRepository.GetUser(request.Username);
            if (user == null)
            {
                return null;
            }
            if (!MatchPasswordHash(request.Password, user.PasswordHash, user.PasswordSalt))
            {
                return null;
            }
            return user;
        }
        public async Task<Response> Login(UserDto request)
        {
            User user = await ChekcUser(request);
            if (user == null)
            {
                response.Status = "401";
                response.Data = new { Title = "Unauthorized" };
                return response;
            }
            var token = CreateToken(user);
            response.Data = new { token };
            response.Status = "success";
            return response;

        }
        public async Task<Response> Register(UserDto request)
        {
            // check uniqueness of user name
            if (_userRepository.GetUser(request.Username)!=null)
            {
                response.Status = "400";
                response.Data = new { Title = "User already exists, please try different user name" };
                return response;
            }

            if (!CheckPasswordStrength(request.Password))
            {
                response.Status = "400";
                response.Data = new { Title = "Password must include uppercase and lowercase and digit and special char and min length 8" };
                return response;
            }
            encryptPassword(request.Password, out byte[] passwordHash, out byte[] passwordKey);
            User user = new User();
            user.Username = request.Username;
            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordKey;
            user.ImageFile = "initial.png";
            // check if error happened will saving
            if (!await _userRepository.Create(user))
            {
                response.Status = "400";
                response.Data = new { Title = "Something went wrong while saving" };
                return response;
            }
            response.Status = "201";
            response.Data = new { Title = "User created" };
            return response;

        }

        public async Task<UserData> GetMe()
        {
            var result = string.Empty;
            if (_httpContextAccessor.HttpContext != null)
            {
                result = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Name);
                if ( await _userRepository.GetUser(result) ==null)
                {
                    return null;

                }

            }
            User user = await _userRepository.GetUser(result);
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
        public async Task<Response> changePassword(string oldPassword, string newPassword)
        {
            var userData = GetMe();
            if (userData == null)
            {
                response.Status = "401";
                response.Data = new { Title = "Unauthorized" };
                return response;
            }
            var username = userData.Result.Name;
            UserDto request = new UserDto();
            request.Username = username;
            request.Password = oldPassword;
            User user = await ChekcUser(request);
           
            if (user == null)
            {
                response.Status = "400";
                response.Data = new { Title = "Wrong Password" };
            }
            if (!CheckPasswordStrength(newPassword))
            {
                response.Status = "400";
                response.Data = new { Title = "Password must include uppercase and lowercase and digit and special char and min length 8" };
                return response;
            }
            encryptPassword(newPassword, out byte[] passwordHash, out byte[] passwordKey);
            User data = new User();
            data.Username = username;
            data.PasswordHash = passwordHash;
            data.PasswordSalt = passwordKey;
            if (!_userRepository.updateUser(data))
            {
                response.Status = "400";
                response.Data = new { Title = "Something went wrong while updating" };
                return response;
            }
            response.Status = "200";
            response.Data = new { Title = "Password change successfully" };
            return response;
        }

        public async Task<int> SaveImage(IFormFile imageFile)
        {
            var userData = GetMe();
            if (userData == null)
            {
                return 401;
            }
            var username = userData.Name;

            var user =await _userRepository.GetUser(username);
            if(user.ImageFile != string.Empty || user.ImageFile!= "initial.png")
            {
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
        private bool MatchPasswordHash(string passwordText, byte[] password, byte[] passwordKey)
        {
            using (var hmac = new HMACSHA512(passwordKey))
            {
                var passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(passwordText));
                // check equality password
                for (int i = 0; i < passwordHash.Length; i++)
                {
                    if (passwordHash[i] != password[i])
                        return false;
                }

                return true;
            }
        }
        public void encryptPassword(string password, out byte[] passwordHash, out byte[] passwordKey)
        {
            // get passwordKey
            using (var hmac = new HMACSHA512())
            {
                passwordKey = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));

            }


        }

    }
}

