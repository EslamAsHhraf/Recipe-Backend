using Authorization.Models;

namespace Authorization.Interfaces
{
    public interface IUserRepository
    {
        // To check login (username and password)
        User Authenticate(string userName, string password);
        // To check Register (username and password)
        bool Register(string userName, string password);
        // check if user exists
        bool UserAlreadyExists(string userName);
        // save change sin data base
        bool Save();
        // get user name
        string GetMyName();
        // check strength of password
        bool CheckPasswordStrength(string password);

    }
}
