using WebApplication1.Interfaces;
using WebApplication1.Models;
using WebApplication1.DBEntities;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace WebApplication1.Services
{
    public class QueryService : IQueryService
    {
        readonly IApplicationDbContext _applicationDbContext;
        readonly IVisitStatisticsService _visitStatisticsService;

        static List<Query> _queriesToProcessList;

        private int _processingTime = 60000; //Переделать как свойство

        public QueryService(IApplicationDbContext applicationDbContext, IConfiguration config, IVisitStatisticsService visitStatisticsService)
        {
            _applicationDbContext = applicationDbContext;
            _visitStatisticsService = visitStatisticsService;

            string processingTimeString = config["Query:ProcessingTime"];
            int processingTime = 60000;

            if (!string.IsNullOrEmpty(processingTimeString) && int.TryParse(processingTimeString, out processingTime))
            {
                _processingTime = processingTime;
            }
            else
            {
                throw new Exception(this.GetType().ToString() + " Exception");
            }

            _queriesToProcessList = AdaptQuery(_applicationDbContext.Queries.Where(q => q.IsDone == false).ToList());

        }

        public async Task ProcessQueryAsync(Query query) //Переделать в возвращающий тип либо сделать сохранение в списке
        {
            AddQueryToProcessingList(query);

            query.QueryInfo.Result = await ExecuteQueryAsync(query);
            
            await SaveQueryResult(query);
        }
        private async Task AddQueryToProcessingList(Query query)
        {
            _applicationDbContext.Queries.Add(new()
            {
                Id = query.QueryId,
            });
            await _applicationDbContext.SaveChanges();

            _queriesToProcessList.Add(query);
        }

        private async Task SaveQueryResult(Query query)
        {
            var queryEntity = _applicationDbContext.Queries.Where(q => q.Id == query.QueryId).Single();
            queryEntity.Result = JsonSerializer.Serialize(query.MakeInfoObject());
            queryEntity.IsDone = true;
            await _applicationDbContext.SaveChanges();
        }

        private async Task HoldQuery(string queryId)
        {
            int updateRate = 1000;

            var query = _queriesToProcessList.Where(q => q.QueryId == queryId).Single();

            var queryInfo = query.QueryInfo;

            int millisecondsToProcess = queryInfo.TimeInMillisecondsToProcess;
            int percentStep = updateRate / millisecondsToProcess * 100;

            await Task.Run(async () =>
            {
                while (query.QueryInfo.Percent < 100)
                {
                    query.QueryInfo.Percent += percentStep;
                    await Task.Delay(updateRate);
                }
                query.QueryInfo.Percent = 100;
            });
        }

        private async Task<object> ExecuteQueryAsync(Query query)
        {
            //var queryEntity = _applicationDbContext.Queries.Where(q => q.UserId == queryId).Single();
            if (!string.IsNullOrEmpty(query.QueryParameters.UserId))
            {
                query.QueryInfo.ProcessingStarted = DateTime.Now;
                query.QueryInfo.ProcessingEndedExpectedTime = query.QueryInfo.ProcessingStarted.AddMilliseconds(_processingTime);

                var TaskHold = HoldQuery(query.QueryId);
                
                var visitStatisticsList = await _visitStatisticsService
                    .FindBetweenAsync(query.QueryParameters.RangeBegin, query.QueryParameters.RangeEnd, query.QueryParameters.UserId);

                //query.QueryInfo.Result = new
                //{
                //    UserId = query.QueryParameters.UserId,
                //    Count_Sign_In = visitStatisticsList.Count.ToString()
                //}; //Обратить внимание

                await TaskHold;

                return new
                {
                    UserId = query.QueryParameters.UserId,
                    Count_Sign_In = visitStatisticsList.Count.ToString()
                };
            }
            else return null;
        }
        
        public Query GetQuery(string queryId)
        {
            return _queriesToProcessList.Where(q => q.QueryId == queryId).Single();
        }

        //public async Task<Query> GetQueryAsync(string queryId)
        //{
        //    return _queriesList.Where(q => q.QueryId == queryId).Single();
        //}
        public List<Query> GetQueriesList()
        {
            return _queriesToProcessList;
        }
        private static List<Query> AdaptQuery(List<QueryEntity> queryEntityList)
        {
            List<Query> queryList = new();
            Query? query;
            foreach (QueryEntity queryEntity in queryEntityList)
            {
                query = new()
                {
                    QueryId = queryEntity.Id,
                    QueryInfo = new(),
                    QueryParameters = new(),
                };
                queryList.Add(query);
            }
            return queryList;
        }
    }
}
