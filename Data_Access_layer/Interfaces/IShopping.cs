using Data_Access_layer.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data_Access_layer.Interfaces
{
    public interface IShopping
    {
        public IEnumerable<Shopping> GetShopping(int id);
        public IEnumerable<Shopping> GetPurchased(int id);
        public  Task<Shopping> check(string searchTerm, int id);

    }
}
