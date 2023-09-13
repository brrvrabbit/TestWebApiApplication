using WebApplication1.Interfaces;
using WebApplication1.Models;
using WebApplication1.DBEntities;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using WebApplication1.DBContext;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace WebApplication1.Services
{
    public class QueryService : IQueryService
    {
        readonly IApplicationDbContext _applicationDbContext;
        readonly IVisitStatisticsService _visitStatisticsService;
        readonly DbContextOptions<AppDbContext> _options;
        readonly IServiceScopeFactory _serviceProviderFactory;
        static List<Query> _queriesToProcessList = new();

        private int _processingTime = 60000; //Переделать как свойство

        public QueryService(IApplicationDbContext applicationDbContext,
            IServiceScopeFactory serviceProviderFactory,
            DbContextOptions<AppDbContext> options,
            IConfiguration config,
            IVisitStatisticsService visitStatisticsService)
        {
            _applicationDbContext = applicationDbContext;
            _visitStatisticsService = visitStatisticsService;
            _options = options;
            _serviceProviderFactory = serviceProviderFactory;

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

        }
        public async Task ProcessQueryAsync(Query query) //Переделать в возвращающий тип либо сделать сохранение в списке
        {
            await AddQueryToProcessingListAsync(query);

            query.QueryInfo.Result = await ExecuteQueryAsync(query);
            
            await SaveQueryResultToDbAsync(query);
        }
        private async Task AddQueryToProcessingListAsync(Query query)
        {
            _applicationDbContext.Queries.Add(new()
            {
                Id = query.QueryId,
                UserId = query.QueryParameters.UserId,
                RangeBegin = query.QueryParameters.RangeBegin,
                RangeEnd = query.QueryParameters.RangeEnd
            });
            await _applicationDbContext.SaveChanges();

            _queriesToProcessList.Add(query);
        }

        private async Task SaveQueryResultToDbAsync(Query query)
        {
            if (query.QueryInfo.Result != null)
            {
                try
                {
                    await using var scope = _serviceProviderFactory.CreateAsyncScope();
                    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                    var queryEntity = context.Queries.Where(q => q.Id == query.QueryId).Single();
                    queryEntity.Result = JsonSerializer.Serialize(query.QueryInfo.Result, new JsonSerializerOptions { WriteIndented = true })  ;
                    queryEntity.IsDone = true;
                    await context.SaveChanges();
                        
                }
                catch (Exception ex)
                {

                }
            }
            else throw new Exception();
        }

        private async Task HoldQueryAsync(string queryId)
        {
            int updateRate = 1000;

            var query = _queriesToProcessList.Where(q => q.QueryId == queryId).Single();

            var queryInfo = query.QueryInfo;

            double millisecondsToProcess = queryInfo.TimeInMillisecondsToProcess;
            double percentStep = updateRate / millisecondsToProcess * 100;

            while (query.QueryInfo.Percent < 100)
            {

                query.QueryInfo.Percent += (int)percentStep;
                await Task.Run(async Task () =>
                {
                    await Task.Delay(updateRate);
                });
            }
            query.QueryInfo.Percent = 100;
        }

        private async Task<object> ExecuteQueryAsync(Query query)
        {
            //var queryEntity = _applicationDbContext.Queries.Where(q => q.UserId == queryId).Single();
            if (!string.IsNullOrEmpty(query.QueryParameters.UserId))
            {
                query.QueryInfo.ProcessingStarted = DateTime.Now;
                query.QueryInfo.ProcessingEndedExpectedTime = query.QueryInfo.ProcessingStarted.AddMilliseconds(_processingTime);

                var TaskHold = HoldQueryAsync(query.QueryId);
                
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
        
        public async Task<Query> GetQuery(string queryId)
        {
            try
            {
                
                if (_queriesToProcessList.Where(q => q.QueryId == queryId).Any())
                {
                    var query = _queriesToProcessList.Where(q => q.QueryId == queryId).Single();
                    return query;
                }
                else
                {
                    var query = _applicationDbContext.Queries.Where(q => q.Id == queryId).Single();
                    return AdaptQuery(query);
                }
            }
            catch(Exception ex)
            {
                return null;
            }
        }

        //public async Task<Query> GetQueryAsync(string queryId)
        //{
        //    return _queriesList.Where(q => q.QueryId == queryId).Single();
        //}
        public async Task<List<Query>> GetQueriesList()
        {
            return _queriesToProcessList;
        }
        private static List<Query> AdaptQueryList(List<QueryEntity> queryEntityList)
        {
            List<Query> queryList = new();
            Query? query;
            foreach (QueryEntity queryEntity in queryEntityList)
            {
                query = new()
                {
                    QueryId = queryEntity.Id,
                    QueryInfo = new()
                    {
                        Percent = Convert.ToInt32(queryEntity.IsDone) * 100,
                        Result = JsonSerializer.Deserialize(queryEntity.Result, typeof(object)),
                        QueryId = queryEntity.Id
                    },
                    QueryParameters = new()
                    {
                        RangeBegin = queryEntity.RangeBegin,
                        RangeEnd = queryEntity.RangeEnd,
                        UserId = queryEntity.UserId
                    }
                };
                queryList.Add(query);
            }
            return queryList;
        }
        private static Query AdaptQuery(QueryEntity queryEntity)
        {
            Query? query;

            query = new()
            {
                QueryId = queryEntity.Id,
                QueryInfo = new()
                {
                    Percent = Convert.ToInt32(queryEntity.IsDone) * 100,
                    Result = JsonSerializer.Deserialize(queryEntity.Result, typeof(object)),
                    QueryId = queryEntity.Id
                },
                QueryParameters = new()
                {
                    RangeBegin = queryEntity.RangeBegin,
                    RangeEnd = queryEntity.RangeEnd,
                    UserId = queryEntity.UserId
                }
            };

            return query;
        }
    }
}
