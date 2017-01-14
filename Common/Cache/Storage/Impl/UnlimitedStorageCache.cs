using Common.Cache.Storage.Generator;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;

namespace Common.Cache.Storage.Impl
{
    public class UnlimitedStorageCache : StorageCacheBase
    {
        public UnlimitedStorageCache(StorageFolder sf, string cacheDirectory, ICacheGenerator cacheFileNameGenerator, long cacheMaxLifetimeInMillis = 0) : base(sf, cacheDirectory, cacheFileNameGenerator, cacheMaxLifetimeInMillis)
        {
        }

        public override Task<bool> SaveAsync(string cacheKey, IRandomAccessStream cacheStream)
        {
            return InternalSaveAsync(CacheFileNameGenerator.GenerateCacheName(cacheKey), cacheStream);
        }
    }
}
