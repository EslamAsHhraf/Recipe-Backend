using Authorization.Interfaces;
using System.Security.Cryptography;
using Data_Access_layer.Data;
using Data_Access_layer.Model;
using Microsoft.AspNetCore.Identity;

namespace Authorization.Repository
{
    public class UserRepository: IUserRepository
    {
        private readonly DataContext _dc;

        public UserRepository(DataContext dc)
        {
            _dc = dc;
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
        public void encryptPassword( string password, out byte[] passwordHash, out byte[] passwordKey)
        {
            // get passwordKey
            using (var hmac = new HMACSHA512())
            {
                passwordKey = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));

            }

          
        }

        public bool Register(string username, string password)
        {
            encryptPassword( password, out byte[] passwordHash, out byte[] passwordKey);
            User user = new User();
            user.Username = username;
            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordKey;
            user.ImageFile = "";
            _dc.Users.Add(user);
            return Save();
        }

        public bool UserAlreadyExists(string userName)
        {
            return  _dc.Users.Any(x => x.Username == userName);
        }
        public bool changePassword(string password,User user)
        {
            encryptPassword(password, out byte[] passwordHash, out byte[] passwordKey);
            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordKey;
            _dc.Update(user);
            return Save();

        }
        public bool Save()
        {
            var saved = _dc.SaveChanges();
            return saved > 0 ? true : false;
        }
      
    }
}

 