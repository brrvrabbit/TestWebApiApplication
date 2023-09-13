using Microsoft.EntityFrameworkCore;
using WebApplication1.DBEntities;

namespace WebApplication1.Interfaces
{
    public interface IApplicationDbContext
    {
        DbSet<UserEntity> Users { get; set; }
        DbSet<QueryEntity> Queries { get; set; }
        DbSet<VisitStatisticsEntity> VisitStatistics { get; set; }

        Task<int> SaveChanges();
    }
}
