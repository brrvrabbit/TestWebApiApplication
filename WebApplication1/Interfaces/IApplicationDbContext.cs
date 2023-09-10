using Microsoft.EntityFrameworkCore;
using WebApplication1.DBEntities;

namespace WebApplication1.Interfaces
{
    public interface IApplicationDbContext
    {
        DbSet<UserEntity> Users { get; set; }
        Task<int> SaveChanges();
    }
}
