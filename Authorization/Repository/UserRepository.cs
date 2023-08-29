using Authorization.Data;
using Authorization.Interfaces;
using Authorization.Models;
using System.Security.Claims;
using System.Security.Cryptography;

namespace Authorization.Repository
{
    public class UserRepository: IUserRepository
    {
        private readonly DataContext _dc;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserRepository(DataContext dc, IHttpContextAccessor httpContextAccessor)
        {
            _dc = dc;
            _httpContextAccessor = httpContextAccessor;

        }
        public User Authenticate(string username, string passwordText)
        {
            var user = _dc.Users.FirstOrDefault(x => x.Username == username);
            // user found or not
            if (user == null || user.PasswordSalt == null)
                return null;

            if (!MatchPasswordHash(passwordText, user.PasswordHash, user.PasswordSalt))
                return null;

            return user;
        }

        private bool MatchPasswordHash(string passwordText, byte[] password, byte[] passwordKey)
        {
            using (var hmac = new HMACSHA512(passwordKey))
            {
                var passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(passwordText));
                // check equality password
                for (int i = 0; i < passwordHash.Length; i++)
                {
                    if (passwordHash[i] != password[i])
                        return false;
                }

                return true;
            }
        }

        public bool Register(string username, string password)
        {
            byte[] passwordHash, passwordKey;
            // get passwordKey
            using (var hmac = new HMACSHA512())
            {
                passwordKey = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));

            }
          
            User user = new User();
            user.Username = username;
            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordKey;

            _dc.Users.Add(user);
            return Save();
        }

        public bool UserAlreadyExists(string userName)
        {
            return  _dc.Users.Any(x => x.Username == userName);
        }
        public string GetMyName()
        {
            var result = string.Empty;
            if (_httpContextAccessor.HttpContext != null)
            {
                result = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Name);
            }
            return result;
        }

        public bool Save()
        {
            var saved = _dc.SaveChanges();
            return saved > 0 ? true : false;
        }
        public bool CheckPasswordStrength(string password)
        {
            if (password.Any(char.IsDigit) && password.Any(char.IsLower) && password.Any(char.IsUpper) && password.Length >= 8 &&
                password.IndexOfAny("!@#$%^&*?_~-£().,".ToCharArray()) != -1)
                return true;
            return false;
        }
    }
}

 