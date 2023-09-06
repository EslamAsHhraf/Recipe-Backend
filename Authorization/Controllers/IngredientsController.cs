using Data_Access_layer.Interfaces;
using Data_Access_layer.Model;
using Microsoft.AspNetCore.Mvc;

namespace RecipeAPI.Controllers
{
    [Route("api/ingredients")]
    public class IngredientsController : Controller
    {
        private readonly IRepository<Ingredient> _ingredientsRepository;

        public IngredientsController(IRepository<Ingredient> ingredientRepository)
        {
            _ingredientsRepository = ingredientRepository;
        }

        [HttpGet]
        public IEnumerable<Ingredient> GetAllIngredients()
        {
            return _ingredientsRepository.GetAll();
        }

    }
}