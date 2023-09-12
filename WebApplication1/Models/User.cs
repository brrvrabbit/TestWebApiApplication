using System.Text.Json.Serialization;
using WebApplication1.DBEntities;

namespace WebApplication1.Models
{
    public class User
    {
        public string Id { get; set; }
        public string Username { get; set; }
        //public List<VisitStatistics> VisitStatistics { get; set; } = new();
        [JsonIgnore]
        public UserEntity UserEntity 
        {
            get
            {
                return new UserEntity()
                {
                    Id = Id,
                    Username = Username,
                };
            }
        }
    }
}
