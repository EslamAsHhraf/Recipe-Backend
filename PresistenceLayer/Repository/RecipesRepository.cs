using PresistenceLayer.Data;
using DomainLayer.Interfaces;
using DomainLayer.Model;
using Microsoft.EntityFrameworkCore;

namespace PresistenceLayer.Repository
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