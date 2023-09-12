using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.DBEntities
{
    public class UserEntity
    {
        [Key]
        public string Id { get; set; }
        public string Username { get; set; }
    }
}
