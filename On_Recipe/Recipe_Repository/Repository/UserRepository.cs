using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using Recipe_Repository.Data;
using Recipe_DataAccess.model;

namespace Recipe_Repository.Repository
{
    public class UserRepository<T> : IUserRepository<T> where T : BaseEntity
    {

        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ApplicationDbContext _dc;
        private readonly DbSet<T> entity;
        public UserRepository(ApplicationDbContext dc, IHttpContextAccessor httpContextAccessor)
        {
            _dc = dc;
            entity = _dc.Set<T>();
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
                var passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(passwordText));
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
                passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));

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
            return _dc.Users.Any(x => x.Username == userName);
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
