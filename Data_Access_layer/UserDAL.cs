using Authorization.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace Data_Access_layer
{
    public class UserDAL
    {
        private IUserRepository _userRepository;
        private IConfiguration _configuration;

        public UserDAL(IConfiguration configuration, IUserRepository userService)
        {
            _userRepository = userService;
            _configuration = configuration;
        }
        public ActionResult<string> GetMe()
        {
            var userName = _userRepository.GetMyName();
            //var userName = User?.Identity?.Name;
            return userName;
        }

    }
}
