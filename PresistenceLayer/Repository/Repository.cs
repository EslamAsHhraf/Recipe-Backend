﻿using DomainLayer.Interfaces;
using PresistenceLayer.Data;
using Microsoft.EntityFrameworkCore;
using DomainLayer.Model;
using System.Reflection;

namespace PresistenceLayer.Repositories
{
    public class Repository<T> : IRepository<T> where T : BaseEntity
    {
        private readonly DataContext _context;
        private readonly DbSet<T> entity;
         private readonly DbSet<Recipe> entityRecipe;
        public Repository(DataContext context)
        {
            _context = context;
            entity = _context.Set<T>();
            entityRecipe = _context.Set<Recipe>();
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

        public async Task<T> Update(T _object)
        {
            _context.Update(_object);
            _context.SaveChanges();
            return _object;
        }


        public async Task<IEnumerable<T>> SearchByName(string searchTerm)
        {

            // Filter the entities by name.
            var filteredEntities = entity.Where(entity => entity.Title.ToLower().Contains(searchTerm.ToLower()));

            return (IEnumerable<T>)filteredEntities;
        }
        public async  Task<bool> CreateList(List<T> _object)
        {
            foreach (var updatedEntity in _object)
            {
                _context.AddAsync(updatedEntity);
            }
             
            return (_context.SaveChanges())>0;
        }
        public async Task<bool> DeleteList(IEnumerable<T> _object)
        {
            foreach (var deletedEntity in _object)
            {
                _context.Remove(deletedEntity);
            }

            return (_context.SaveChanges()) > 0;
        }

        public async Task<T> GetByName(string searchTerm)
        {

            // Filter the entities by name.
            return  await entity.FirstOrDefaultAsync(entity => entity.Title.ToLower()==(searchTerm.ToLower()));

            
        }
    }
}