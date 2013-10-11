using Microsoft.ApplicationServer.Caching;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cache
{
    public class AzureCache : IAzureCache
    {
        private DataCache cache;
        public AzureCache()
        {
            DataCacheFactory factory = new DataCacheFactory();
            cache = factory.GetDefaultCache();
        }

        public void PutItem(CacheItem item)
        {
            cache.CreateRegion(item.FileId);
            cache.Add(Guid.NewGuid().ToString(), item, item.FileId);
        }

        public List<CacheItem> GetItems(string fileId)
        {
            List<CacheItem> items = new List<CacheItem>();
            var cacheItems = cache.GetObjectsInRegion(fileId);
            foreach (var item in cacheItems)
            {
                items.Add(item.Value as CacheItem);
            }
            return items;
        }

        public void RemoveItems(string fileId)
        {
            cache.RemoveRegion(fileId);
        }
    }
}
