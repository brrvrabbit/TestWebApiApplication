using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApplication1.DBContext;
using WebApplication1.DBEntities;
using WebApplication1.Interfaces;
using WebApplication1.Models;

namespace WebApplication1.Tests
{
    public class TestDbContext : DbContext, IApplicationDbContext
    {
        public TestDbContext(DbContextOptions<TestDbContext> options) : base(options)
        {
            Database.EnsureCreated();
        }
        public DbSet<UserEntity> Users { get; set; }
        public DbSet<QueryEntity> Queries { get; set; }
        public DbSet<VisitStatisticsEntity> VisitStatistics { get; set; }
        public async Task<int> SaveChanges()
        {
            return await base.SaveChangesAsync();
        }
        public (List<UserEntity>, List<VisitStatisticsEntity>) GenerateDummyData()
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
        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    var dummyData = GenerateDummyData();
        //    modelBuilder.Entity<UserEntity>().HasData(dummyData.Item1);
        //    modelBuilder.Entity<VisitStatisticsEntity>().HasData(dummyData.Item2);
        //}

       
    }
}
