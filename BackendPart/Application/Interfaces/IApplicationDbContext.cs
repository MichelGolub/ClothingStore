using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IApplicationDbContext
    {
        public DbSet<Mark> Marks { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Property> Properties { get; set; }
        public DbSet<Shop> Shops { get; set; }
        public DbSet<Type> Types { get; set; }
        public DbSet<Value> Values { get; set; }
        public DbSet<UserConnection> Connections { get; set; } 
        Task<int> SaveChangesAsync();
    }
}
