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
            response.Status = "200";
            response.Data= userName;
            return response;
        }




        [HttpPost("register")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<Response> Register(UserDto request)
        {
            // check if ModelState is valid
            if (!ModelState.IsValid)
            {
                response.Status = "500";
                response.Data = "";
                return response;
            }
            // check uniqueness of user name
            if (_userRepository.UserAlreadyExists(request.Username))
            {
                //ModelState.AddModelError("Title", "User already exists, please try different user name");
                //return BadRequest(ModelState);
                response.Status = "500";
                response.Data = "User already exists, please try different user name";
                return response;
            }

            if (!_userRepository.CheckPasswordStrength(request.Password))
            {
                //ModelState.AddModelError("Title", "Password must include uppercase and lowercase and digit and specail char and min length 8");
                response.Status = "500";
                response.Data = "Password must include uppercase and lowercase and digit and specail char and min length 8";
                return response;
            }

            // check if error happened will saving
            if (!_userRepository.Register(request.Username, request.Password, request.image))
            {
                //ModelState.AddModelError("", "Something went wrong while saving");
                response.Status = "500";
                response.Data = "Something went wrong while saving";
                return response;
            }
            // create user successfully
            response.Status = "200";
            response.Data = "Successfully";
            return response;
        }







        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<Response> Login(UserDto request)
        {
            // check if model isn't correct
            if (!ModelState.IsValid)
            {
                response.Status = "500";
                response.Data = "Invalid model";
                return response;
            }
            // get user
            var user = _userRepository.Authenticate(request.Username, request.Password);

            if (user == null)
            { 
                response.Status = "500";
                response.Data = "Invalid user name or password";
                return response;
            }
            // create token
            string token = CreateToken(user);
            response.Status = "200";
            response.Data = token;
            return response;
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
            return jwt;
        }
    }
}
