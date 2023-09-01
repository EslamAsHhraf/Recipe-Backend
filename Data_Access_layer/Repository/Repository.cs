using Data_Access_layer.Interfaces;
using Data_Access_layer.Data;
using Microsoft.EntityFrameworkCore;
using Data_Access_layer.Model;

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

        public T GetById(int Id)
        {
            return entity.FirstOrDefault(x => x.Id == Id);
        }

        public void Update(T _object)
        {
            _context.Update(_object);
            _context.SaveChanges();
        }



       
    }
}