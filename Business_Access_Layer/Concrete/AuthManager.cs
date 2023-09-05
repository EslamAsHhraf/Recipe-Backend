using Authorization.Interfaces;
using Business_Access_Layer.Abstract;
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

namespace Business_Access_Layer.Concrete
{
    public class AuthManager: IAuthService
    {
        private IUserRepository _userRepository;
        public readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuthManager(IUserRepository userRepository, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _userRepository = userRepository;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;


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

        public string GetMyName()
        {
            var result = string.Empty;
            if (_httpContextAccessor.HttpContext != null)
            {
                result = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Name);
                if (_userRepository.UserAlreadyExists(result))
                {
                    return result;

                }
                result = null;

            }
            return result;
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
                _httpContextAccessor.HttpContext.Response.Cookies.Delete("token");
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
            var username = GetMyName();
            if(username == null)
            {
                status = "fail";
                title = "Token not found";
                code = 401;
                return;
            }
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

    }
}
