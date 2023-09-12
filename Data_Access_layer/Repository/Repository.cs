using Data_Access_layer.Interfaces;
using Data_Access_layer.Data;
using Microsoft.EntityFrameworkCore;
using Data_Access_layer.Model;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;

namespace Data_Access_layer.Repositories
{
    public class Repository<T> : IRepository<T> where T : BaseEntity
    {
        private readonly DataContext _context;
        private readonly DbSet<T> entity;
        public Repository(DataContext context)
        {
            _context = context;
            entity = _context.Set<T>();
        }

        public async Task<T> Create(T _object)
        {
            await _context.AddAsync(_object);
            await _context.SaveChangesAsync();

            return _object;
        }

        public void Delete(T _object)
        {
            _context.Remove(_object);
            _context.SaveChanges();
        }

        public IEnumerable<T> GetAll()
        {
            return entity.AsEnumerable();
        }

        public async Task<T> GetById(int Id)
        {
            return await entity.FirstOrDefaultAsync(x => x.Id == Id);
        }

        public async Task<T> Update(T _object)
        {
            _context.Update(_object);
            _context.SaveChanges();
            return _object;
        }


        public async Task<IEnumerable<T>> SearchByName(string searchTerm)
        {

            // Filter the entities by name.
            var filteredEntities = entity.Where(entity => entity.Title.ToLower().Contains(searchTerm.ToLower()));

            return (IEnumerable<T>)filteredEntities;
        }


    }
}