using Data_Access_layer.Model;

namespace Authorization.Interfaces
{
    public interface IUserRepository
    {
        // To check login (username and password)
        User Authenticate(string username, string password);
        // To check Register (username and password)
        bool Register(string username, string password);
        // check if user exists
        bool UserAlreadyExists(string username);
        // save change sin data base
        bool Save();
        bool changePassword(string password, User user);
        void encryptPassword( string password, out byte[] passwordHash, out byte[] passwordKey);
        public User GetUser(string username);
        public bool updateUser(User user);

    }
}
