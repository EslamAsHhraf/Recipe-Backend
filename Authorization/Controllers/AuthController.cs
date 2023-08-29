using Authorization.Models;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;
using Authorization.Interfaces;

namespace Authorization.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        public static User user= new User();
        public readonly IConfiguration _configuration;
        private readonly IUserRepository _userRepository;


        public AuthController(IConfiguration configuration, IUserRepository userService)

        {
            _userRepository = userService;
            _configuration = configuration;
        }
        [HttpGet, Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]

        public ActionResult<string> GetMe()
        {
            var userName = _userRepository.GetMyName();
            //var userName = User?.Identity?.Name;
            return Ok(userName);
        }

        [HttpPost("register")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<User> Register(UserDto request)
        {
            // check if ModelState is valid
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            // check uniqueness of user name
            if (_userRepository.UserAlreadyExists(request.Username))
            {
                ModelState.AddModelError("Title", "User already exists, please try different user name");
                return BadRequest(ModelState);
            }

            if (!_userRepository.CheckPasswordStrength(request.Password))
            {
                ModelState.AddModelError("Title", "Password must include uppercase and lowercase and digit and specail char and min length 8");
                return BadRequest(ModelState);
            }

            // check if error happened will saving
            if (!_userRepository.Register(request.Username, request.Password))
            {
                ModelState.AddModelError("", "Something went wrong while saving");
                return StatusCode(500, ModelState);
            }
            // create user successfully
            return StatusCode(201);
        }

        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<User> Login(UserDto request)
        {
            // check if model isn't correct
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            // get user
            var user = _userRepository.Authenticate(request.Username, request.Password);

            if (user == null)
            {
                ModelState.AddModelError("Title", "Invalid user name or password");
                return BadRequest(ModelState);
            }
            // create token
            string token = CreateToken(user);
            return Ok(token);
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
