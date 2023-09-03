using Authorization.Model;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;
using Authorization.Interfaces;
using Data_Access_layer.Model;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Security.Cryptography;
using IdentityServer4.Models;
using Microsoft.AspNetCore.Http;
using Azure.Core;
using System.Net;
using System.Collections.Generic;

namespace Authorization.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        public readonly IConfiguration _configuration;
        private readonly IUserRepository _userRepository;

        private Business_Access_Layer.UserBAL _BAL;


        public AuthController(IConfiguration configuration, IUserRepository userService)

        {
            _userRepository = userService;
            _configuration = configuration;
            _BAL = new Business_Access_Layer.UserBAL(configuration, userService);
        }
        [AllowAnonymous]
        [HttpGet("koko")] // <-- notice the difference
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> AuthorizeAsync()
        {
            //generating access token ommited for brevity
            //Response.Headers.Add("Set-Cookie", "cookie_name=cookie_value; expires=Wed, 21 Oct 2023 07:28:00 GMT; path=/");

            Response.Cookies.Append("token", "eyJhbGciOiJodHRwOi8vd3d3LnczLm9yZy8yMDAxLzA0L3htbGRzaWctbW9yZSNobWFjLXNoYTUxMiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoic3RyaW5nIiwiaHR0cDovL3NjaGVtYXMueG1sc29hcC5vcmcvd3MvMjAwNS8wNS9pZGVudGl0eS9jbGFpbXMvbmFtZWlkZW50aWZpZXIiOiIzIiwiaHR0cDovL3NjaGVtYXMubWljcm9zb2Z0LmNvbS93cy8yMDA4LzA2L2lkZW50aXR5L2NsYWltcy9yb2xlIjoiVXNlciIsImV4cCI6MTY5MzgzNDQzMX0.QeDYzZo7-pPc42s4iQBHEdR0Byep9-7uf8fNkc6_JqM-TvUKEq0cV5hydX4l1eO4wRtJBB4wdYq8uPRY361YcA",
                  new CookieOptions
                  {
                      Expires = DateTime.Now.AddDays(5),
                      HttpOnly = false,
                      Secure = true,
                      IsEssential = true,
                      SameSite = SameSiteMode.None
                  });
            
            return Ok(); ;
        }
      
        [HttpGet, Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<string> GetMe()
        {

            var name = _BAL.GetMe();
         
  //          Response.Headers.Add("Access-Control-Allow-Origin", Request.Headers["Origin"]);
  //          Response.Headers.Add("Access-Control-Allow-Credentials", "true");
  //          Response.Headers.Add(
  //  "Access-Control-Allow-Headers",
  //  "Origin, X-Requested-With, Content-Type, Accept"
  //);
  //          Response.Headers.Add("Access-Control-Allow-Methods", "GET, POST, OPTIONS, PUT, DELETE");
            return Ok(new { nameee = name });
           
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
        public ActionResult<User> Login([FromBody] UserDto request)
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

            var res = CreateToken(user);
            //Response.Cookies.Append("X-Access-Token", token, new CookieOptions() { HttpOnly = true, SameSite = SameSiteMode.Strict });
            //HttpContext.Response.Cookies.Append("Token-Expired", token, new CookieOptions { HttpOnly = true });
            //using (var client = new HttpClient())
            //{
            //    client.DefaultRequestHeaders.Add("Authorization", "Bearer" + token);
            //}
            //SetTokenInCookie(token, request.Username);
            //var res = {
            //        "token":token
            // };
            //HttpContext.Response.Cookies.Append("token", token,
            //             new Microsoft.AspNetCore.Http.CookieOptions { Expires = DateTime.Now.AddDays(1) });
            return Ok(res);
        }
        private dynamic CreateToken(User user)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.NameIdentifier,user.Id.ToString()),
                new Claim(ClaimTypes.Role, "User")
            };
            var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(
                _configuration.GetSection("AppSettings:Token").Value!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
            
            var token = new JwtSecurityToken(
                claims: claims,
                expires:DateTime.Now.AddDays(1),
                signingCredentials:creds
                );
            var jwt = new JwtSecurityTokenHandler().WriteToken(token);
            SetJWT(jwt);
            return new { token = jwt };
        }
        private dynamic JWTGeneratorEslam(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration.GetSection("AppSettings:Token").Value!);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] { new Claim("id", user.Username), new Claim(ClaimTypes.Role, "User")
                        }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha512Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var encrypterToken = tokenHandler.WriteToken(token);

            SetJWT(encrypterToken);

          

            return new { token = encrypterToken, username = user.Username };
        }
       
        private void SetJWT(string encrypterToken)
        {

            HttpContext.Response.Cookies.Append("token", encrypterToken,
                  new CookieOptions
                  {
                      Path = "/",
                      Expires = DateTime.Now.AddDays(5),
                      HttpOnly = false,
                      Secure = true,
                      IsEssential = true,
                      Domain = "http://localhost:4200/",
                      SameSite = SameSiteMode.None
                  });
        }
        private void SetTokenInCookie(string token,string username)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, username), // Set user information
                new Claim("CustomTokenClaim", token) // Add your token as a custom claim
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true // You can set whether the cookie is persistent
            };

            HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);

        }
    }
}
