using System.Security.Cryptography;
using PresistenceLayer.Data;
using DomainLayer.Model;
using DomainLayer.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;

namespace PresistenceLayer.Repository
{
    public class UserRepository<T> : IUserRepository<T> where T : User
    {
        private readonly DataContext _dc;
        private readonly DbSet<T> entity;

        public UserRepository(DataContext dc)
        {
            _dc = dc;
            entity = _dc.Set<T>();

        }
        public async Task<T> GetUser(string username)
        {
            return await entity.FirstOrDefaultAsync(x => x.Username == username);
        }
       
        public async Task<T> GetById(int Id)
        {
            return await entity.FirstOrDefaultAsync(x => x.Id == Id); ;
        }
        public async Task<bool> Create(T _object)
        {
            await _dc.AddAsync(_object);

            return Save();
        }
        
        public bool updateUser(T _object)
        {
            _dc.Update(_object);
            return Save();

        }
        public bool Save()
        {
            var saved = _dc.SaveChanges();
            return saved > 0 ? true : false;
        }
      
    }
}

 