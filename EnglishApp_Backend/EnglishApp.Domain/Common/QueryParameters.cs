using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnglishApp.Domain.Common
{
    public class QueryParameters
    {
        public int First { get; set; } = 0;
        public int Rows { get; set; } = 10;
        public int? SortOrder { get; set; }
        public Dictionary<string, FilterCriteria>? Filters { get; set; }
        public string? GlobalFilter { get; set; }
        public List<SortMeta>? MultiSortMeta { get; set; }
    }

    public class FilterCriteria
    {
        public dynamic? Value { get; set; }
        public string? MatchMode { get; set; }
    }

    public class SortMeta
    {
        public string? Field { get; set; }
        public int? Order { get; set; }
    }
}
