using Authorization.Models;

namespace Authorization.Interfaces
{
    public interface IUserRepository
    {
        User Authenticate(string userName, string password);
        bool Register(string userName, string password);

        bool UserAlreadyExists(string userName);
        bool Save();
        string GetMyName();
        bool CheckPasswordStrength(string password);

    }
}
