using PresistenceLayer.Data;
using DomainLayer.Interfaces;
using DomainLayer.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PresistenceLayer.Repository
{
    public class ShoppingRepository :IShopping
    {
        private readonly DbSet<Shopping> entity;
        private readonly DataContext _context;

        public ShoppingRepository(DataContext context)
        {
            _context = context;
            entity = _context.Set<Shopping>();
        }

        public IEnumerable<Shopping> GetShopping(int id)
        {
            var filteredEntities = entity.Where(entity => entity.CreatedBy == id && entity.QuantityShopping!=0).ToList();

            return filteredEntities;
        }

        public IEnumerable<Shopping> GetPurchased(int id)
        {
            var filteredEntities = entity.Where(entity => entity.CreatedBy == id && entity.QuantityPurchased != 0).ToList();

            return filteredEntities;
        }
        public async Task<Shopping> check(string searchTerm, int id)
        {

            // Filter the entities by name.
            return await entity.FirstOrDefaultAsync(entity => entity.Title.ToLower() == (searchTerm.ToLower()) && entity.CreatedBy ==id);


        }

    }
}
