using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cache
{
    public interface IAzureCache
    {
        void PutItem(CacheItem item);
        List<CacheItem> GetItems(string fileId);
        void RemoveItems(string fileId);
        
    }
}
