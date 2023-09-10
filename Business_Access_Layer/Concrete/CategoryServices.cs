
using Business_Access_Layer.Abstract;
using Data_Access_layer.Interfaces;
using Data_Access_layer.Model;


namespace Business_Access_Layer.Concrete
{

    public class CategoryServices : ICategory
    {
        private readonly IRepository<Category> _categoryRepository;

        public CategoryServices(IRepository<Category> categoryRepository)
        {
            _categoryRepository = categoryRepository;

        }
        public IEnumerable<Category> GetCategories()
        {
            return _categoryRepository.GetAll();
        }
    }
}

