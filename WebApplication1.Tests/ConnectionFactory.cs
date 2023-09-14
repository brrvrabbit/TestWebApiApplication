using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApplication1.DBContext;

namespace WebApplication1.Tests
{
    public class ConnectionFactory : IDisposable 
    {
        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        public TestDbContext CreateContextForInMemory()
        {
            var option = new DbContextOptionsBuilder<TestDbContext>().UseInMemoryDatabase(databaseName: "Test_Database").Options;

            var context = new TestDbContext(option);
            if (context != null)
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();
            }

            return context;
        }

        public TestDbContext CreateContextForSQLite()
        {
            var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();

            var option = new DbContextOptionsBuilder<TestDbContext>().UseSqlite(connection).Options;

            var context = new TestDbContext(option);

            if (context != null)
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();
            }

            return context;
        }


        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
        #endregion
    }

}

