using Business_Access_Layer.Abstract;
using Data_Access_layer.Interfaces;
using Data_Access_layer.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business_Access_Layer.Concrete
{
    public class RecipesServices: IRecipesServices
    {
        private readonly IRecipes _recipesRepository;
        private IAuthService _userService;

        public RecipesServices(IRecipes recipesRepository, IAuthService userService)
        {
            _recipesRepository = recipesRepository;
            _userService= userService;
        }
        public IEnumerable<Recipe> GetMyRecipes()
        {
            UserData data=_userService.GetMe();
            if(data==null)
            {
                return null;
            }
            return _recipesRepository.GetMyRecipes(data.Id);
        }
    }
}
