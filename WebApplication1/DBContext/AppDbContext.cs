using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using WebApplication1.DBEntities;
using WebApplication1.Interfaces;
using WebApplication1.Models;

namespace WebApplication1.DBContext
{
    public class AppDbContext : DbContext, IApplicationDbContext
    {
        bool autodelete = true;
        public AppDbContext (DbContextOptions<AppDbContext> options) : base(options)
        {
            //if(autodelete) Database.EnsureDeleted();
            //Database.EnsureCreated();
        }
        public DbSet<UserEntity> Users { get; set; }
        public DbSet<QueryEntity> Queries { get; set; }
        public DbSet<VisitStatisticsEntity> VisitStatistics { get; set; }

        public new async Task<int> SaveChanges()
        {
            return await base.SaveChangesAsync();
        }

        private (List<UserEntity>,List<VisitStatisticsEntity>) GenerateDummyData()
        {
            int usersCount = 15,
            minVisit = 1,
            maxVisit = 5;
            Random r = new Random();

            List<UserEntity> userEntities = new();
            User user;

            List<VisitStatisticsEntity> visitStatisticsEntities = new();
            VisitStatistics visitStatistics;
            int id = 1; 

            for (int i = 0; i < usersCount; i++)
            {
                int visitCount = r.Next(1, 100);
                user = new()
                {
                    Id = Guid.NewGuid().ToString(),
                    Username = "user" + r.Next().ToString(),
                };
                
                for (int j = 0; j < visitCount; j++)
                {
                    visitStatistics = new()
                    {
                        Id = id,
                        UserId = user.Id,
                        Datetime = DateTime.Now.Subtract(TimeSpan.FromHours(r.Next(24)))
                    };
                    visitStatisticsEntities.Add(visitStatistics.VisitStatisticsEntity);

                    id++;
                }
                userEntities.Add(user.UserEntity);
            }
            return (userEntities, visitStatisticsEntities);
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //var dummyData = GenerateDummyData();
            //modelBuilder.Entity<UserEntity>().HasData(dummyData.Item1);
            //modelBuilder.Entity<VisitStatisticsEntity>().HasData(dummyData.Item2);
        }

        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    var connectionStringBuilder = new SqliteConnectionStringBuilder { DataSource = "MyDb.db" };
        //    var connectionString = connectionStringBuilder.ToString();
        //    var connection = new SqliteConnection(connectionString);

        //    optionsBuilder.UseSqlite(connection);
        //}
    }
}
