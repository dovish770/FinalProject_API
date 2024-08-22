using Microsoft.EntityFrameworkCore;
using System.Data.Common;
using PojectFinal___API.Modles;

namespace PojectFinal___API.Services
{
    public class DbConnection : DbContext
    {
        public DbConnection(DbContextOptions<DbConnection> options) : base(options)
        {
            Database.EnsureCreated();
        }

        public DbSet<Target> targets { get; set; }
        public DbSet<Agent> agents { get; set; }
        public DbSet<Mission> missions { get; set; }        
    }
}
