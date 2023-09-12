using WebApplication1.Models;

namespace WebApplication1.Interfaces
{
    public interface IVisitStatisticsService
    {
        List<VisitStatistics> GetVisitStatistics();
        Task<List<VisitStatistics>> GetVisitStatisticsAsync();
        List<VisitStatistics> FindBetween(DateTime start, DateTime end, string userId);
        Task<List<VisitStatistics>> FindBetweenAsync(DateTime start, DateTime end, string userId);
    }
}
