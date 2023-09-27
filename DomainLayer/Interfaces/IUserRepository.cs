using DomainLayer.Model;

namespace DomainLayer.Interfaces
{
    public interface IUserRepository<T> where T : User
    {
        // To check login (username and password)
        //T Authenticate(string username, string password);
        // To check Register (username and password)
        public Task<bool> Create(T _object);

        //bool Register(string username, string password);
        // check if user exists
        //bool UserAlreadyExists(string username);
        // save change sin data base
        bool Save();
        //bool changePassword(string password, T user);
        //void encryptPassword(string password, out byte[] passwordHash, out byte[] passwordKey);
        public Task<T> GetUser(string username);
        public Task<T> GetById(int Id);
        public bool updateUser(T user);
    }
}
