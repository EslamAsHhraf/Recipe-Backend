using DomainLayer.Model;
using Business_Access_Layer.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business_Access_Layer.Abstract
{
    public interface ICategory
    {
        public Task<Response> GetCategories();
        public Task<Response> GetCategoryById(int Id);
        public Task<Response> PostCategory(Category category);

    }
}
