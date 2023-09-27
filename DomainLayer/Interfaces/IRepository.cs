
using DomainLayer.Model;

namespace DomainLayer.Interfaces
{
    public interface IRepository<T> where T : BaseEntity
    {
        public Task<T> Create(T _object);
        public void Delete(T _object);
        public Task<T> Update(T _object);
        public IEnumerable<T> GetAll();
        public Task<T> GetById(int Id);
        public Task<IEnumerable<T>> SearchByName(string searchTerm);
        public Task<bool> CreateList(List<T> _object);
        public Task<bool> DeleteList(IEnumerable<T> _object);
        public Task<T> GetByName(string searchTerm);
    }
}
