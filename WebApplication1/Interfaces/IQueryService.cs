using Microsoft.AspNetCore.Mvc.ModelBinding;
using WebApplication1.Models;

namespace WebApplication1.Interfaces
{
    public interface IQueryService
    {
        Query GetQuery(string queryGuid);
        List<Query> GetQueriesList();
        //Task<Query> GetQueryAsync(string queryGuid);
        //Task<QueryInfo> ExecuteQueryAsync(Query query);
        Task ProcessQueryAsync(Query query);
    }
}
