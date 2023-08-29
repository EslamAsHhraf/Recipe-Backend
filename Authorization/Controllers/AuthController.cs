using Authorization.Models;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;
using System.Security.Cryptography;
using Authorization.Interfaces;

namespace Authorization.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        public static User user= new User();
        public readonly IConfiguration _configuration;
        private readonly IUserRepository _userService;

        public AuthController(IConfiguration configuration, IUserRepository userService)

        {
            _userService = userService;
            _configuration = configuration; 

        }
        [HttpGet, Authorize]
        public ActionResult<string> GetMe()
        {
            var userName = _userService.GetMyName();
            //var userName = User?.Identity?.Name;
            return Ok(userName);
        }

        [HttpPost("register")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<User> Register(UserDto request)
        {
            //string passwordHash=
            //    BCrypt.Net.BCrypt.HashPassword(request.Password);  
            //user.Username = request.Username;
            //user.PasswordHash = passwordHash;
            //return Ok(user);

            // check if ModelState is valid
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            // check uniqueness of user name
            if (_userService.UserAlreadyExists(request.Username))
            {
                ModelState.AddModelError("Title", "User already exists, please try different user name");
                return BadRequest(ModelState);
            }
            // check if error happened will saving
            if (!_userService.Register(request.Username, request.Password))
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
            //if(user.Username != request.Username) {
            //    return BadRequest("User not found.");
            //}
            //if(!BCrypt.Net.BCrypt.Verify(request.Password,user.PasswordHash))
            //{
            //    return BadRequest("Wrong Password.");

            //}
            //string token = CreateToken(user);


            //return Ok(token);
            //if (user.Username != request.Username)
            //{
            //    return BadRequest("User not found.");
            //}

            //if (!VerifyPasswordHash(request.Password, user.PasswordHash, user.PasswordSalt))
            //{
            //    return BadRequest("Wrong password.");
            //}

            //string token = CreateToken(user);

            //return Ok(token);
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var user = _userService.Authenticate(request.Username, request.Password);

            if (user == null)
            {
                ModelState.AddModelError("Title", "Invalid user name or password");
                return BadRequest(ModelState);
            }

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
        //private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        //{
        //    using (var hmac = new HMACSHA512())
        //    {
        //        passwordSalt = hmac.Key;
        //        passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
        //    }
        //}


        //private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        //{
        //    using (var hmac = new HMACSHA512(passwordSalt))
        //    {
        //        var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
        //        return computedHash.SequenceEqual(passwordHash);
        //    }
        //}
    }
}
