using Microsoft.AspNetCore.Http;

namespace Data_Access_layer.Model
{
    public class RecipeData
    {
        public Recipe recipe { get; set; }
        public IFormFile imageFile { get; set; }
    }
}
