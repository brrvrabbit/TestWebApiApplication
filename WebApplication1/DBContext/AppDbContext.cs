﻿using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using WebApplication1.DBEntities;
using WebApplication1.Interfaces;
using WebApplication1.Models;

namespace WebApplication1.DBContext
{
    public class AppDbContext : DbContext, IApplicationDbContext
    {
        public AppDbContext (DbContextOptions<AppDbContext> options) : base(options)
        {
            if(Database.EnsureCreated()) Database.Migrate();
        }
        public DbSet<UserEntity> Users { get; set; }
        public DbSet<QueryEntity> Queries { get; set; }
        public DbSet<VisitStatisticsEntity> VisitStatistics { get; set; }

        public new async Task<int> SaveChanges()
        {
            return await base.SaveChangesAsync();
        }

        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    modelBuilder.Entity<User>().HasData(
        //        new User 
        //        { 
        //            Id = 
        //        });
        //}

        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    var connectionStringBuilder = new SqliteConnectionStringBuilder { DataSource = "MyDb.db" };
        //    var connectionString = connectionStringBuilder.ToString();
        //    var connection = new SqliteConnection(connectionString);

        //    optionsBuilder.UseSqlite(connection);
        //}
    }
}
