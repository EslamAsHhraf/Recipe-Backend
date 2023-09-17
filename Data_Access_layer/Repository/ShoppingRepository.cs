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

     
    }
}
