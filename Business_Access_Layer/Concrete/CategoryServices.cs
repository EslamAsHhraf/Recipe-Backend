using Business_Access_Layer.Abstract;
using DomainLayer.Interfaces;
using DomainLayer.Model;
using Business_Access_Layer.Common;

namespace Business_Access_Layer.Concrete
{

    public class CategoryServices : ICategory
    {
        private readonly IRepository<Category> _categoryRepository;
        private Response response = new Response();

        public CategoryServices(IRepository<Category> categoryRepository)
        {
            _categoryRepository = categoryRepository;

        }
        public async Task<Response> GetCategories()
        {
            var Categories= _categoryRepository.GetAll();
            if (Categories == null)
            {
                response.Status = "204";
                response.Data = new { Title = "No Content" };
                return response;
            }
            response.Status = "200";
            response.Data = Categories;
            return response;
        }
        public async Task<Response> GetCategoryById(int Id)
        {
            var category = await _categoryRepository.GetById(Id);
            if (category == null)
            {
                response.Status = "404";
                response.Data = new { Title = "Not Found" };
                return response;
            }
            response.Status = "200";
            response.Data = category;
            return response;
        }
        public async Task<Response> PostCategory(Category cat)
        {
            var category = await _categoryRepository.Create(cat);
            if (category == null)
            {
                response.Status = "404";
                response.Data = new { Title = "Not Found" };
                return response;
            }
            response.Status = "200";
            response.Data = category;
            return response;
        }

    }
}

