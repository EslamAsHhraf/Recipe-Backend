using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Recipe_DataAccess;
using Recipe_DataAccess.model;
using Recipe_Repository.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Recipe_Repository.Repository
{
    public class Repository<T> : IRepository<T> where T : BaseEntity
    {
        private readonly ApplicationDbContext _datac;
        private readonly DbSet<T> entity;
        public Repository(ApplicationDbContext dc, IHttpContextAccessor httpContextAccessor)
        {
            _datac = dc;
            entity = _datac.Set<T>();
        }

        public IEnumerable<T> GetAll()
        {
            return entity.AsEnumerable();
        }

        public T GetById(int Id)
        {
            return entity.FirstOrDefault(x => x.Id == Id);
        }
    }
}
