using Recipe_DataAccess;
using Recipe_DataAccess.model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Recipe_Repository.Repository
{
    public interface IRepository<T> where T : BaseEntity
    {
        IEnumerable<T> GetAll();

        T GetById(int Id);

    }
}
