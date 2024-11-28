using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Helpers
{
    public class QueryComment
    {
        public string? Title { get; set; } = null;
        public DateTime? CreatedOn { get; set; } = null;
        public commentSort SortBy { get; set; }
        public bool IsDescending { get; set; } = false;
    }
    public enum commentSort
    {
        Id = 0,
        Title = 1,
        CreatedOn = 2,
    }
}