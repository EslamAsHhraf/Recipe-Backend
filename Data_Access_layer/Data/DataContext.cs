using Data_Access_layer.Model;
using Microsoft.EntityFrameworkCore;

namespace Data_Access_layer.Data
{
    public class DataContext : DbContext
    {
        public DataContext()
        {
        }
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
        public DbSet<User> Users { get; set; }
        public DbSet<Recipe> Recipes { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Favourite> Favourites { get; set; }
        public DbSet<Rating> Ratinges { get; set; }
        public DbSet<Category> Categorys { get; set; }
        public DbSet<Ingredient> Ingredients { get; set; }
        public DbSet<RecipeIngredients> RecipeIngredients { get; set; }

    }
}
