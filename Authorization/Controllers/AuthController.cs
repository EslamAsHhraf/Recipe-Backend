using Authorization.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using DomainLayer.Model;
using Business_Access_Layer.Abstract;
using System.ComponentModel.DataAnnotations;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Business_Access_Layer.Authorization;
using static System.Net.Mime.MediaTypeNames;
using Business_Access_Layer.Common;

namespace Authorization.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        public static User user = new User();
        private Response response = new Response();
        private IAuthService _userService;


        public AuthController( IAuthService userService)

        {
            _userService = userService;
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<Response>> GetUserById(int id)
        {

            var User = _userService.GetUserById(id);
            if (User == null)
            {
                response.Status = "401";
                response.Data = new { Title = "Cannot find user" };
                return StatusCode(401, response);
            }
       
            response.Status = "200";
            response.Data = new {
                name = User.Item1,
                id = User.Item2,
                image = User.Item3
            };
           
            return StatusCode(200, response);

        }
        [HttpGet("me")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]

        public async  Task<ActionResult<Response>> GetMe()
        {
            var data =  await _userService.WhoLogin();
            return StatusCode(Int16.Parse(data.Status), data);

        }

        [HttpGet("logout")]
        [ProducesResponseType(StatusCodes.Status200OK)]

        public ActionResult<Response> Logout()
        {
            var data = _userService.logout();
            
            return StatusCode(Int16.Parse(data.Result.Status), data.Result);

        }

        [HttpPost("register")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<Response> Register(UserDto request)
        {
            // check if ModelState is valid
            if (!ModelState.IsValid)
            {
                response.Status = "400";
                response.Data = new { Title = "User Name and Password are required" };
                return StatusCode(400, response);
            }

            var data = _userService.Register(request);
            return StatusCode(Int16.Parse(data.Result.Status), data.Result);
    
        }


        [HttpGet("login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Response>> Login([FromQuery] UserDto request)
        {
            // check if model isn't correct
            if (request==null || request.Username == null || request.Password ==null)
            {
                response.Status = "400";
                response.Data = new { Title = "User Name and Password are required" };
                return StatusCode(400, response);
            }
            // check for user
            var data = await  _userService.Login(request);
            return StatusCode(Int16.Parse(data.Status), data);
        }

        [HttpPut("changePassword")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public ActionResult<Response> ChangePassword([Required] string oldPassword, [Required] string newPassword )
        {
            if (!ModelState.IsValid)
            {
            // Handle validation errors
            return BadRequest(ModelState);
            }

            var data = _userService.changePassword(oldPassword, newPassword);
            return StatusCode(Int16.Parse(data.Result.Status), data.Result);
        }

        [HttpPut("updateImage")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> UpdateImage( IFormFile ImageFile)
        {
            if (ImageFile == null)
            {
                response.Status = "404";
                response.Data = new { Title = "ImageFile is null" };
                return StatusCode(404, response); ;
            }

            var data = _userService.SaveImage(ImageFile);
            return StatusCode(Int16.Parse(data.Result.Status), data.Result);
        }

    }}

