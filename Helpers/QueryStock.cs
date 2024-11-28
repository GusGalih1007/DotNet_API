using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Helpers
{
    public class QueryStock
    {
        public string? Symbol { get; set; } = null;
        public string? CompanyName { get; set; } = null;
        public string? Industry { get; set; } = null;
        public Item SortBy { get; set; }
        public bool IsDescending { get; set; } = false;
    }
    public enum Item 
    {
        Id = 1 ,
        Symbol = 2,
        CompanyName = 3,
        Purchase = 4,
        LastDiv = 5,
        Industry = 6,
        MarketCap = 7
    }
}