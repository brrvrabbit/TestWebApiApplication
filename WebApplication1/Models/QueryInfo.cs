using System.Text.Json;
using System.Text.Json.Serialization;

namespace WebApplication1.Models
{
    public class QueryInfo
    {
        [JsonPropertyName("query_guid")]
        public string QueryId { get; set; } = string.Empty;
        [JsonPropertyName("percent")]
        public int Percent {  get; set; } = 0;
        [JsonPropertyName("result")]
        public object? Result { get; set; } = null;
        [JsonIgnore]
        public DateTime ProcessingStarted { get; set; }
        [JsonIgnore]
        public DateTime ProcessingEnded { get; set; }
        [JsonIgnore]
        public DateTime ProcessingEndedExpectedTime { get; set; }
        [JsonIgnore]
        public double TimeInMillisecondsToProcess  //Пересмотреть или убрать
        { 
            get
            { 
                return ProcessingEndedExpectedTime.Subtract(ProcessingStarted).TotalMilliseconds; 
            } 
        }
    }
}
