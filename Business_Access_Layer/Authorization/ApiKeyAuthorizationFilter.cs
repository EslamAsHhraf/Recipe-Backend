using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Claims;
using Data_Access_layer.Interfaces;
using Data_Access_layer.Model;
using Microsoft.EntityFrameworkCore;
using Data_Access_layer.Data;
using Microsoft.AspNetCore.Http;
using Business_Access_Layer.Common;
using Newtonsoft.Json;

namespace Business_Access_Layer.Authorization
{
    public class ApiKeyMiddleware
    {
        //private readonly IUserRepository<User> _userRepository;
        private readonly RequestDelegate _next;
        private Response response = new Response();

        public ApiKeyMiddleware(RequestDelegate next )
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context, DataContext dbContext )
        {
            //string apiKey = context.HttpContext.Request.Headers[ApiKeyHeaderName];
            var result = string.Empty;
            if (context.Request.HttpContext != null)
            {
                result = context.Request.HttpContext.User.FindFirstValue(ClaimTypes.Name);
                if(result == null)
                {
                    response.Status = "401";
                    response.Data = new { Title = "Unauthorized" };

                    var customResponse = JsonConvert.SerializeObject(response);
                    context.Response.Headers.Add("Content-Type", "application/json");
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    await context.Response.WriteAsync(customResponse);
                    return;
                }
                var check = dbContext.Users.Any(u => u.Username == result);
                if (!check)
                {
                    response.Status = "401";
                    response.Data = new { Title = "Unauthorized" };

                    var customResponse = JsonConvert.SerializeObject(response);
                    context.Response.Headers.Add("Content-Type", "application/json");
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    await context.Response.WriteAsync(customResponse);
                    return;

                }
               
            }
            await _next(context);

        }
    }
}
