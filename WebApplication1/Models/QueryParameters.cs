using Microsoft.AspNetCore.Routing.Constraints;

namespace WebApplication1.Models
{
    public class QueryParameters
    {
        private DateTime rangeBegin;
        private DateTime rangeEnd;
        public DateTime RangeBegin { get; set; }
        public DateTime RangeEnd 
        { 
            get 
            {
                return rangeEnd;
            }
            set 
            { 
                if(value >= RangeBegin)
                {
                    rangeEnd = value;
                }
            }
        }
        public string? UserId { get; set; }
    }
}
