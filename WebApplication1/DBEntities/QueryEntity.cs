using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using WebApplication1.Models;
using System.Text.Json.Serialization;

namespace WebApplication1.DBEntities
{
    public class QueryEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string Id { get; set; } = string.Empty;
        public bool IsDone { get; set; } = false;
        public string? Result { get; set; } = null;
        public DateTime RangeBegin { get; set; }
        public DateTime RangeEnd { get; set; }
        public string? UserId { get; set; }
        //public DateTime ProcessingStarted { get; set; }
        //public DateTime ProcessingEnded { get; set; }
        //public int Percent { get; set; } = 0;
    }
}
