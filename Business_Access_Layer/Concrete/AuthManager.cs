using Business_Access_Layer.Abstract;
using Authorization.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Hosting;
using System.Security.Cryptography;
using DomainLayer.Interfaces;
using DomainLayer.Model;
using Business_Access_Layer.Common;
using Microsoft.Extensions.Hosting;


namespace Business_Access_Layer.Concrete
{
    public class AuthManager : IAuthService
    {
        private readonly IUserRepository<User> _userRepository;
        private readonly IFileServices _fileServices;
        public readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;

        private Response response = new Response();

        public AuthManager(IUserRepository<User> userRepository, IConfiguration configuration, IHttpContextAccessor httpContextAccessor, IFileServices fileServices)
        {
            _userRepository = userRepository;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
            _fileServices = fileServices;
        }
        public async Task<User> CheckUser(UserDto request)
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
            User user = await CheckUser(request);
            if (user == null)
            {
                response.Status = "400";
                response.Data = new { Title = "Invalid username or password" };
                return response;
            }
            var token = CreateToken(user);
            response.Data = new { token };
            response.Status = "200";
            return response;

        }
        public async Task<Response> Register(UserDto request)
        {
            // check uniqueness of user name
            if (await _userRepository.GetUser(request.Username) != null)
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
            user.ImageFile = "https://firebasestorage.googleapis.com/v0/b/imagenet-5a741.appspot.com/o/images%2Finitial.jpg?alt=media&token=0c87e207-4db8-496c-93e9-bb396a17771f&_gl=1*kvi8pc*_ga*MTA4MjAxMzE5My4xNjkyMzQ2NDYx*_ga_CW55HF8NVT*MTY5NjAxMTExNS41LjEuMTY5NjAxNTU2MC4yMy4wLjA.";
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
        public async Task<Response> WhoLogin()
        {
            var UserData = await GetMe();
            if (UserData == null)
            {
                response.Data = new { Title = "Token not found" };
                response.Status = "401";
                return response;
            }
            else
            {

                response.Status = "200";
                response.Data = UserData;
                return response;
            }
        }
        public async Task<UserData> GetMe()
        {
            var result = string.Empty;
            if (_httpContextAccessor.HttpContext != null)
            {
                result = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Name);

            }
            User user = await _userRepository.GetUser(result);
            if (user == null)
            {
                return null;
            }
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
        public async Task<Response> logout()
        {
            if (_httpContextAccessor.HttpContext.Request.Cookies["token"] != null)
            {
                _httpContextAccessor.HttpContext.Response.Cookies.Delete("token", new CookieOptions
                {
                    Secure = true,
                    SameSite = SameSiteMode.None
                });
                response.Status = "200";
                response.Data = new { Title = "Token Deleted successfully" };
                return response;
            }
            response.Status = "401";
            response.Data = new { Title = "Token Not found" };
            return response;

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
            var userData = await GetMe();
            if (userData == null)
            {
                response.Status = "401";
                response.Data = new { Title = "Untheorized User" };
                return response;
            }
            var username = userData.Name;
            UserDto request = new UserDto();
            request.Username = username;
            request.Password = oldPassword;
            User user = await CheckUser(request);

            if (user == null)
            {
                response.Status = "400";
                response.Data = new { Title = "Wrong Password" };
                return response;
            }
            if (!CheckPasswordStrength(newPassword))
            {
                response.Status = "400";
                response.Data = new { Title = "Password must include uppercase and lowercase and digit and special char and min length 8" };
                return response;
            }
            encryptPassword(newPassword, out byte[] passwordHash, out byte[] passwordKey);

            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordKey;
            if (!_userRepository.updateUser(user))
            {
                response.Status = "400";
                response.Data = new { Title = "Something went wrong while updating" };
                return response;
            }
            response.Status = "200";
            response.Data = new { Title = "Password change successfully" };
            return response;
        }

        public async Task<Response> SaveImage(IFormFile imageFile)
        {
            var userData = GetMe();
            if (userData == null)
            {
                response.Status = "401";
                response.Data = new { Title = "Untheorized User" };
                return response;
            }
            var username = userData.Result.Name;

            var user = await _userRepository.GetUser(username);

            string image = await _fileServices.SaveImage(imageFile, user.Username + user.Id);
            if (image == "")
            {
                response.Status = "400";
                response.Data = new { Title = "Error while update image" };
                return response;
            }
            user.ImageFile = image;
            if (!_userRepository.updateUser(user))
            {
                response.Status = "400";
                response.Data = new { Title = "Error while update image" };
                return response;

            }
            response.Status = "200";
            response.Data = user;
            return response;
        }

        public bool MatchPasswordHash(string passwordText, byte[] password, byte[] passwordKey)
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

        public Tuple<string, int, string> GetUserById(int Id)
        {
            var user = _userRepository.GetById(Id);

            // Check if user is null or user.Result is null before accessing properties
            if (user == null || user.Result == null)
            {
                return null;
            }

            // Access properties after ensuring user and user.Result are not null
            return Tuple.Create(user.Result.Username, user.Result.Id, user.Result.ImageFile);
        }

    }
}

