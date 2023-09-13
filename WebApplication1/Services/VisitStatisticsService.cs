using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Net.NetworkInformation;
using WebApplication1.DBEntities;
using WebApplication1.Interfaces;
using WebApplication1.Models;

namespace WebApplication1.Services
{
    public class VisitStatisticsService : IVisitStatisticsService
    {
        readonly IApplicationDbContext _applicationDbContext;


        public VisitStatisticsService(IApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;

        }

        public List<VisitStatistics> FindBetween(DateTime start, DateTime end, string userId)
        {
            return AdaptVisitStatistics(_applicationDbContext.VisitStatistics
                .Where(v => (v.UserId == userId) && (v.Datetime >= start) && (v.Datetime <= end))
                .ToList<VisitStatisticsEntity>());
        }

        public async Task<List<VisitStatistics>> FindBetweenAsync(DateTime start, DateTime end, string userId)
        {
            return AdaptVisitStatistics(await _applicationDbContext.VisitStatistics
                .Where(v => (v.UserId == userId) && (v.Datetime >= start) && (v.Datetime <= end))
                .ToListAsync<VisitStatisticsEntity>());
        }

        public List<VisitStatistics> GetVisitStatistics()
        {
            return AdaptVisitStatistics(_applicationDbContext.VisitStatistics
                .ToList<VisitStatisticsEntity>());
        }

        public async Task<List<VisitStatistics>> GetVisitStatisticsAsync()
        {
            return AdaptVisitStatistics(await _applicationDbContext.VisitStatistics
                .ToListAsync<VisitStatisticsEntity>());
        }

        private static List<VisitStatistics> AdaptVisitStatistics(List<VisitStatisticsEntity> visitStatisticsEntities)
        {
            List<VisitStatistics> visitStatisticsList = new();
            VisitStatistics? visitStatistics;
            foreach (VisitStatisticsEntity visitStatisticsEntity in visitStatisticsEntities)
            {
                visitStatistics = new()
                {
                    Id = visitStatisticsEntity.Id,
                    Datetime = visitStatisticsEntity.Datetime,
                    UserId = visitStatisticsEntity.UserId,
                };
                visitStatisticsList.Add(visitStatistics);
            }
            return visitStatisticsList;
        }

    }
}
