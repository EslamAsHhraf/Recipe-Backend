using Authorization.Model;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;
using Authorization.Interfaces;
using Data_Access_layer.Model;
using RecipeAPI.Common;
using System.Net;
using Microsoft.AspNetCore.Http.HttpResults;
using Nest;

namespace Authorization.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        public static User user= new User();
        public readonly IConfiguration _configuration;
        private readonly IUserRepository _userRepository;
        private Response response = new Response();


        public AuthController(IConfiguration configuration, IUserRepository userService)

        {
            _userRepository = userService;
            _configuration = configuration;
        }

        [HttpGet, Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]

        public ActionResult<Response> GetMe()
        {
            var userName = _userRepository.GetMyName();
            response.Data = userName;
            response.Status = "success";
            return StatusCode(200, response);
        }




        [HttpPost("register")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<Response> Register(UserDto request)
        {
            // check if ModelState is valid
            if (!ModelState.IsValid)
            {
                response.Status = "fail";
                response.Data = new { Title = "User Name and Password are required" };
                return StatusCode(400, response);
            }
            // check uniqueness of user name
            if (_userRepository.UserAlreadyExists(request.Username))
            {
                response.Status = "fail";
                response.Data = new { Title = "User already exists, please try different user name"};
                return StatusCode(400, response);
            }

            if (!_userRepository.CheckPasswordStrength(request.Password))
            {
                response.Status = "fail";
                response.Data = new { Title = "Password must include uppercase and lowercase and digit and special char and min length 8" };
                return StatusCode(400, response);
            }

            // check if error happened will saving
            if (!_userRepository.Register(request.Username, request.Password, request.image))
            {
                response.Status = "fail";
                response.Data = new { Title = "Something went wrong while saving" };
                return StatusCode(400, response);
            }
            // create user successfully
            response.Status = "success";
            return StatusCode(201, response);
        }







        [HttpGet("login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<Response> Login([FromQuery] UserDto request)
        {
            // check if model isn't correct
            if (!ModelState.IsValid)
            {
                response.Status = "fail";
                response.Data = new { Title = "User Name and Password are required" };
                return StatusCode(400, response);
            }
            // get user
            var user = _userRepository.Authenticate(request.Username, request.Password);

            if (user == null)
            {
                response.Status = "fail";
                response.Data = new { Title = "Invalid user name or password" };
                return StatusCode(404, response);;
            }
            // create token
            string token = CreateToken(user);
            response.Data = new { token };
            response.Status = "success";
            return StatusCode(200, response);
        }
        private string CreateToken(User user)
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
                expires:DateTime.Now.AddDays(1),
                signingCredentials:creds
                );
            var jwt = new JwtSecurityTokenHandler().WriteToken(token);
            SetJWT(jwt);
            return jwt;
        }
        private void SetJWT(string encrypterToken)
        {

            Response.Cookies.Append("token", encrypterToken,
                  new CookieOptions
                  {
                      Expires = DateTime.Now.AddDays(5),
                      HttpOnly = false,
                      Secure = true,
                      IsEssential = true,
                      SameSite = SameSiteMode.None
                  });
        }
    }
}
