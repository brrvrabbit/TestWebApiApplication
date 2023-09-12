using System.Text.Json.Serialization;
using WebApplication1.DBEntities;

namespace WebApplication1.Models
{
    public class VisitStatistics
    {
        public int Id { get; set; }
        public DateTime Datetime { get; set; }
        public string UserId { get; set; }
        //public User? User { get; set; }

        [JsonIgnore]
        public VisitStatisticsEntity VisitStatisticsEntity
        {
            get 
            {
                return new()
                {
                    Datetime = Datetime,
                    UserId = UserId
                };
            }
        }
    }
}
