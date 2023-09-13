using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.DBEntities
{
    public class VisitStatisticsEntity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public DateTime Datetime { get; set; }
        public string UserId { get; set; }
        //public UserEntity? User { get; set; }
    }
}
