namespace WebApplication1.Models
{
    public class Query
    {
        public string QueryId { get; set; } = string.Empty;
        public QueryInfo? QueryInfo { get; set; } = null;
        public QueryParameters? QueryParameters { get; set; } = null;
        
        public object? MakeInfoObject()
        {
            var obj = new
            {
                query = QueryId,
                percent = QueryInfo.Percent,
                result = QueryInfo.Result
            };
            return obj;
        }


    }
}