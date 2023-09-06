﻿using Authorization.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Data_Access_layer.Model;
using RecipeAPI.Common;
using Business_Access_Layer.Abstract;
using Azure.Core;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;

namespace Authorization.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        public static User user= new User();
        public readonly IConfiguration _configuration;
        private Response response = new Response();
        private IAuthService _userService;


        public AuthController(IConfiguration configuration, IAuthService userService)

        {
            _configuration = configuration;
            _userService = userService;
        }

        [HttpGet,Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]

        public ActionResult<Response> GetMe()
        {
            var UserData = _userService.GetMe();
            if (UserData == null)
            {
                response.Data = new { Title = "Token not found" };
                response.Status = "fail";
                return StatusCode(401, response);
            }
            else
            {
                response.Data = new { UserData };
                response.Status = "success";
                return StatusCode(200, response);
            }
        }

        [HttpGet("logout")]
        [ProducesResponseType(StatusCodes.Status200OK)]

        public ActionResult<Response> Logout()
        {
            if (_userService.logout())
            {
                response.Data = new { Title = "Token Deleted successfully" };
                response.Status = "success";
                return StatusCode(200, response);
            }
            else
            {

                response.Data = new { Title = "Token not found" };
                response.Status = "fail";
                return StatusCode(401, response);
            }

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
            string status = "", title = "";
            _userService.Register(request, out status, out  title);
            response.Status = status;
            response.Data = new { Title = title };
            if (status == "success")
            {
                return StatusCode(201, response);
            }
            else
            {
                return StatusCode(400, response);

            }

            
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
            //var user = _userRepository.Authenticate(request.Username, request.Password);
            string token = "";
            var user = _userService.Login(request,out token);

            if (user == null )
            {
                response.Status = "fail";
                response.Data = new { Title = "Invalid user name or password" };
                return StatusCode(404, response);;
            }
            // create token
            response.Data = new { token };
            response.Status = "success";
            return StatusCode(200, response);
        }

        [HttpPut("ChangePassword")]
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
            int code = 0;
            string status = "", title = "";
            _userService.changePassword(oldPassword, newPassword, out status, out title, out code);
            response.Status = status;
            response.Data = new { Title = title };
            return StatusCode(code, response);
        }

        [HttpPut("UpdateImage")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> UpdateImage( IFormFile ImageFile)
        {
            if (ImageFile == null)
            {
                response.Status = "fail";
                response.Data = new { Title = "ImageFile is null" };
                return StatusCode(404, response); ;
            }
            Task<int> code =_userService.SaveImage(ImageFile);
            int result = await code;
            if (result == 401)
            {
                response.Status = "fail";
                response.Data = new { Title = "Cookies not found" };
                return StatusCode(404, response); ;
            }
            if (result == 400)
            {
                response.Status = "fail";
                response.Data = new { Title = "Error in saving" };
                return StatusCode(400, response); ;
            }
            response.Status = "success";
            response.Data = new { Title = "Saving new image" };
            return StatusCode(201, response); ;
        }

    }
}
