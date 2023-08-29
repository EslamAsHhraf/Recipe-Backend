using Data_Access_layer.Model;
using Microsoft.EntityFrameworkCore;

namespace Data_Access_layer.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }

    }
}
