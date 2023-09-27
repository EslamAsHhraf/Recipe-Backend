using Microsoft.AspNetCore.Http;

namespace DomainLayer.Model
{
    public class RecipeData
    {
        public Recipe recipe { get; set; }
        public IFormFile imageFile { get; set; }
    }
}
