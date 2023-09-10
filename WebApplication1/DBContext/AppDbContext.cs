using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using WebApplication1.DBEntities;
using WebApplication1.Interfaces;

namespace WebApplication1.DBContext
{
    public class AppDbContext : DbContext, IApplicationDbContext
    {
        public AppDbContext (DbContextOptions<AppDbContext> options) : base(options)
        {

        }
        public DbSet<UserEntity> Users { get; set; }

        public new async Task<int> SaveChanges()
        {
            return await base.SaveChangesAsync();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var connectionStringBuilder = new SqliteConnectionStringBuilder { DataSource = "MyDb.db" };
            var connectionString = connectionStringBuilder.ToString();
            var connection = new SqliteConnection(connectionString);

            optionsBuilder.UseSqlite(connection);
        }
    }
}
