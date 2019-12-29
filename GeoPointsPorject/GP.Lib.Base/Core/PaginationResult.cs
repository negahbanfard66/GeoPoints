using System;
using System.Collections.Generic;
using System.Text;

namespace GP.Lib.Base.Core
{
    public class PaginationResult<T>
    {
        public PaginationResult(int totalCount) => TotalCount = totalCount;

        public int TotalCount { get; set; }
        public IEnumerable<T> Items { get; set; }
    }
}
