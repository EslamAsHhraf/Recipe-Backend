using Data_Access_layer.Interfaces;
using Data_Access_layer.Data;
using Microsoft.EntityFrameworkCore;
using Data_Access_layer.Model;
using Microsoft.AspNetCore.Mvc;

namespace Data_Access_layer.Repositories
{
    public class Repository<T> : IRepository<T> where T : BaseEntity
    {
        private readonly DataContext _context;
        private readonly DbSet<T> entity;
        public Repository(DataContext context)
        {
            _context = context;
            entity = _context.Set<T>();
        }

        public async Task<T> Create(T _object)
        {
            await _context.AddAsync(_object);
            await _context.SaveChangesAsync();

            return _object;
        }

        public void Delete(T _object)
        {
            _context.Remove(_object);
            _context.SaveChanges();
        }

        public IEnumerable<T> GetAll()
        {
            return entity.AsEnumerable();
        }

        public async Task<T> GetById(int Id)
        {
            return await entity.FirstOrDefaultAsync(x => x.Id == Id);
        }

        public void Update(T _object)
        {
            _context.Update(_object);
            _context.SaveChanges();
        }



        public async Task<IEnumerable<T>> SearchByName(string searchTerm)
        {
            // Get all entities of type Recipe from the database.
            var recipes = await _context.Set<T>().ToListAsync();

            // Filter the entities by name.
            var filteredRecipes = recipes.Where(recipe =>
            {
                // Check if the title is not null.
                if (recipe.Title != null)
                {
                    // Get the lowercase title.
                    var lowercaseTitle = recipe.Title.ToLower();

                    // Check if the lowercase title contains the search term.
                    return lowercaseTitle.Contains(searchTerm.ToLower());
                }
                else
                {
                    // The title is null, so return false.
                    return false;
                }
            });

            // Check if there are any recipes that match the search term.
            if (filteredRecipes.Any())
            {
                // Return the recipe.
                return filteredRecipes;
            }
            else
            {
                // Return null if there are no recipes that match the search term.
                return null;
            }
        }

        public  List<Recipe> GetRecipesByIds(List<int> ingredients)
        {
            var allrecipes = new List<Recipe>();
            foreach (var ingredient in ingredients)
            {
                var recipeIngredients =  _context.RecipeIngredients.Where(ri => ri.IngredientId == ingredient).ToList();
                foreach (var recipeIngredient in recipeIngredients)
                {
                    var recipe =  _context.Recipes.Find(recipeIngredient.RecipeId);
                    allrecipes.Add(recipe);
                }
            }
            return allrecipes;
        }

       
    }
}