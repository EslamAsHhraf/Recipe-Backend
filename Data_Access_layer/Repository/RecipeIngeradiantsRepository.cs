using Data_Access_layer.Data;
using Data_Access_layer.Interfaces;
using Data_Access_layer.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data_Access_layer.Repository
{
    public class RecipeIngredientsRepository: IRecipeIngeradiants
    {
        private readonly DataContext _context;
        public RecipeIngredientsRepository(DataContext context)
        {
            _context = context;
        }
        public async Task<RecipeIngredients> Create(RecipeIngredients recipeIngredients)
        {
             _context.Add(recipeIngredients);
             _context.SaveChangesAsync();

            return recipeIngredients;
        }
        public bool Save()
        {
            var saved = _context.SaveChanges();
            return saved > 0 ? true : false;
        }
    }
}
