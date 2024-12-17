using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Helpers
{
    public class QueryComment
    {
        public string? Title { get; set; } = null;
        public string? Symbol { get; set; } = null;
        public bool IsDescending { get; set; } = true;
    }
}