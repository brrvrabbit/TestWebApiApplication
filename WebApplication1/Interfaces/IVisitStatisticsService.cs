using WebApplication1.Models;

namespace WebApplication1.Interfaces
{
    public interface IVisitStatisticsService
    {
        List<VisitStatistics> GetVisitStatistics();
        Task<List<VisitStatistics>> GetVisitStatisticsAsync();
        List<VisitStatistics> FindInRange(DateTime start, DateTime end, string userId);
        Task<List<VisitStatistics>> FindInRangeAsync(DateTime start, DateTime end, string userId);
    }
}
