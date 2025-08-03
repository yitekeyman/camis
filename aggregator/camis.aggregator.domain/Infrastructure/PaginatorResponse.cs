using System.Collections.Generic;

namespace camis.aggregator.domain.Infrastructure
{
    public class PaginatorResponse<T>
    {
        public int TotalSize;
        public ICollection<T> Items;
    }
}