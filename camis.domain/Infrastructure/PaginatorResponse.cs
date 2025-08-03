using System.Collections.Generic;

namespace intapscamis.camis.domain.Infrastructure
{
    public class PaginatorResponse<T>
    {
        public int TotalSize;
        public ICollection<T> Items;
    }
}