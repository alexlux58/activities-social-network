using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Core
{
    public class PagingParams
    {
        private const int MaxPageSize = 50;
        public int PageNumber { get; set; } = 1; // default value
        private int _pageSize = 5; // default value
        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = (value > MaxPageSize) ? MaxPageSize : value; // if value > MaxPageSize, then _pageSize = MaxPageSize, else _pageSize = value
        }

        // public string Sort { get; set; }
        // public string Search { get; set; }

        // public bool IsPagingEnabled => PageSize != 0; // if PageSize != 0, then IsPagingEnabled = true, else IsPagingEnabled = false

        // public string Predicate { get; set; }

        // public string Username { get; set; }

        // public string StartDate { get; set; }

        // public string EndDate { get; set; }

        // public string OrderBy { get; set; }

        // public string OrderByDescending { get; set; }

        // public string OrderByAscending { get; set; }

        // public string OrderByDescendingThenBy { get; set; }


    }
}