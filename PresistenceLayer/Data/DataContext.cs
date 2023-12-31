﻿using DomainLayer.Model;
using Microsoft.EntityFrameworkCore;

namespace PresistenceLayer.Data
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
        public DbSet<RecipeIngredients> RecipeIngredients { get; set; }
        public DbSet<PlanMeals> PlanMeals { get; set; }

        public DbSet<Shopping> Shopping { get; set; }

    }
}
