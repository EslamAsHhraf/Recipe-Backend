using Data_Access_layer.Data;
using Data_Access_layer.Interfaces;
using Data_Access_layer.Model;
using Microsoft.EntityFrameworkCore;
using Nest;

namespace Data_Access_layer.Repository
{
    public class RecipesRepository : IRecipes
    {
        private readonly DbSet<Recipe> entity;
        private readonly DataContext _context;

        public RecipesRepository(DataContext context)
        {
            _context = context;
            entity = _context.Set<Recipe>();
        }
        public IEnumerable<Recipe> GetMyRecipes(int id)
        {
            // Filter the entities by name.
            var filteredEntities = entity.Where(entity => entity.CreatedBy == id);

            return filteredEntities;
        }

    }
}