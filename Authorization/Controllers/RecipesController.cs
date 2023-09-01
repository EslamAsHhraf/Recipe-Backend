using Data_Access_layer.Interfaces;
using Data_Access_layer.Model;
using Microsoft.AspNetCore.Mvc;

namespace RecipeAPI.Controllers
{
    [Route("api/recipes")]
    public class RecipesController : Controller
    {
        private readonly IRepository<Recipe> _recipeRepository;

        public RecipesController(IRepository<Recipe> recipeRepository)
        {
            _recipeRepository = recipeRepository;
        }

        [HttpGet]
        public IEnumerable<Recipe> GetAllRecipes()
        {
            return _recipeRepository.GetAll();
        }

        [HttpGet("{id}")]
        public Recipe GetRecipeById(int id)
        {
            return _recipeRepository.GetById(id);
        }
    }
}
