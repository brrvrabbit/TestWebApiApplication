using Microsoft.AspNetCore.Mvc.ModelBinding;
using WebApplication1.Models;

namespace WebApplication1.Interfaces
{
    public interface IQueryService
    {
        Task<Query> GetQueryAsync(string queryGuid);
        Task<List<Query>>  GetQueriesListAsync();
        //Task<Query> GetQueryAsync(string queryGuid);
        //Task<QueryInfo> ExecuteQueryAsync(Query query);
        Task ProcessQueryAsync(Query query);
    }
}
