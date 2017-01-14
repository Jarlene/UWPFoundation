using Common.Cache.Storage.Generator;
using Common.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;
namespace Common.Cache.Storage.Impl
{
    public class LimitedStorageCache : StorageCacheBase
    {
        private readonly IDictionary<string, long> lastAccessTimeDictionary = new SynchronizedDictionary<string, long>();

        private readonly object lockObject = new object();

        private long _currentCacheSizeInBytes = -1;

        private readonly long cacheLimitInBytes = 1000*1000*1000;
        protected long CurrentCacheSizeInBytes
        {
            get
            {
                lock (lockObject)
                {
                    return _currentCacheSizeInBytes;
                }
            }

            set
            {
                lock (lockObject)
                {
                    _currentCacheSizeInBytes = value;
                }
            }
        }

        public LimitedStorageCache(StorageFolder sf, string cacheDirectory, ICacheGenerator cacheFileNameGenerator, long cacheMaxLifetimeInMillis = DefaultCacheMaxLifetimeInMillis) : base(sf, cacheDirectory, cacheFileNameGenerator, cacheMaxLifetimeInMillis)
        {
            this._currentCacheSizeInBytes = cacheLimitInBytes;
            BeginCountCurrentCacheSize();
        }

        private void BeginCountCurrentCacheSize()
        {
            Task.Factory.StartNew(async () =>
            {
                IReadOnlyList<StorageFile> cacheFiles;
                try
                {
                    var storageFolder = await SF.GetFolderAsync(CacheDirectory);
                    cacheFiles = await storageFolder.GetFilesAsync();
                }
                catch (Exception ex)
                {
                    LogUtils.Log(ex.Message);
                    return;
                }

                long cacheSizeInBytes = 0;

                foreach (var cacheFile in cacheFiles)
                {
                    var properties = await cacheFile.GetBasicPropertiesAsync();
                    var fullCacheFilePath = cacheFile.Name;
                    try
                    {
                        cacheSizeInBytes += (long)properties.Size;
                        lastAccessTimeDictionary.Add(fullCacheFilePath, properties.DateModified.DateTime.Milliseconds());
                    }
                    catch
                    {
                        LogUtils.Log("[error] can not get cache's file size: " + fullCacheFilePath);
                    }
                }
                CurrentCacheSizeInBytes += cacheSizeInBytes; // Updating current cache size
            });
        }


        public override async Task<bool> SaveAsync(string cacheKey, IRandomAccessStream cacheStream)
        {
            var fullFileName = Path.Combine(CacheDirectory, CacheFileNameGenerator.GenerateCacheName(cacheKey));
            var cacheSizeInBytes = cacheStream.AsStreamForRead().Length;

            while (CurrentCacheSizeInBytes + cacheSizeInBytes > cacheLimitInBytes)
            {
                if (!await RemoveOldestCacheFile())
                {
                    break; // All cache deleted
                }
            }

            var wasFileSaved = await base.InternalSaveAsync(fullFileName, cacheStream);

            if (wasFileSaved)
            {
                lastAccessTimeDictionary[Path.Combine(CacheDirectory, fullFileName)] = DateTimeUtils.CurrentTimeMillis();
                CurrentCacheSizeInBytes += cacheSizeInBytes; // Updating current cache size
            }

            return wasFileSaved;
        }

        private async Task<bool> RemoveOldestCacheFile()
        {
            if (lastAccessTimeDictionary.Count == 0) return false;

            var oldestCacheFilePath = lastAccessTimeDictionary.Aggregate((pair1, pair2) => (pair1.Value < pair2.Value) ? pair1 : pair2).Key;

            if (string.IsNullOrEmpty(oldestCacheFilePath)) return false;

            oldestCacheFilePath = Path.Combine(CacheDirectory, oldestCacheFilePath);

            try
            {
                long fileSizeInBytes;
                var storageFile = await SF.GetFileAsync(oldestCacheFilePath);
                var properties = await storageFile.GetBasicPropertiesAsync();
                fileSizeInBytes = (long)properties.Size;

                try
                {
                    await storageFile.DeleteAsync();
                    lastAccessTimeDictionary.Remove(oldestCacheFilePath);
                    CurrentCacheSizeInBytes -= fileSizeInBytes; // Updating current cache size
                    LogUtils.Log("[delete] cache file " + oldestCacheFilePath);
                    return true;
                }
                catch
                {
                    LogUtils.Log("[error] can not delete oldest cache file: " + oldestCacheFilePath);
                }
            }
            catch (Exception ex)
            {
                LogUtils.Log("[error] can not get olders cache's file size: " + oldestCacheFilePath + " exception: " + ex.Message);
            }

            return false;
        }
    }
}
