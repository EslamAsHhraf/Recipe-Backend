
using Data_Access_layer.Model;

namespace Data_Access_layer.Interfaces
{
    public interface IRepository<T> where T : BaseEntity
    {
        public Task<T> Create(T _object);
        public void Delete(T _object);
        public void Update(T _object);
        public IEnumerable<T> GetAll();
        public T GetById(int Id);
    }
}
