using Common.Utils;
using System;
using System.Threading.Tasks;
using System.IO;
using Windows.Storage;
using Windows.Storage.Streams;
using Common.Cache.Storage.Generator;

namespace Common.Cache.Storage
{
    public abstract class StorageCacheBase
    {
        protected const long DefaultCacheMaxLifetimeInMillis = 7 * 24 * 60 * 60 * 1000; // == 604800000;

        protected readonly StorageFolder SF;

        protected virtual string CacheDirectory { get; set; }

        protected virtual ICacheGenerator CacheFileNameGenerator { get; set; }

        protected virtual long CacheMaxLifetimeInMillis { get; set; }


        protected StorageCacheBase(StorageFolder sf, string cacheDirectory, ICacheGenerator cacheFileNameGenerator, long cacheMaxLifetimeInMillis = DefaultCacheMaxLifetimeInMillis)
        {
            if (sf == null)
            {
                throw new ArgumentNullException("isf");
            }
            if (string.IsNullOrEmpty(cacheDirectory))
            {
                throw new ArgumentException("cacheDirectory name could not be null or empty");
            }
            if (cacheDirectory.StartsWith("\\"))
            {
                throw new ArgumentException("cacheDirectory name shouldn't starts with double slashes: \\");
            }
            if (cacheFileNameGenerator == null)
            {
                throw new ArgumentNullException("cacheFileNameGenerator");
            }
            SF = sf;
            CacheDirectory = cacheDirectory;
            CacheFileNameGenerator = cacheFileNameGenerator;
            CacheMaxLifetimeInMillis = cacheMaxLifetimeInMillis;
            // Creating cache directory if it not exists
            SF.CreateFolderAsync(CacheDirectory).AsTask();
        }

        public abstract Task<bool> SaveAsync(string cacheKey, IRandomAccessStream cacheStream);

        protected async virtual Task<bool> InternalSaveAsync(string fullFilePath, IRandomAccessStream cacheStream)
        {
            var storageFile = await SF.CreateFileAsync(fullFilePath, CreationCollisionOption.ReplaceExisting);
            using (IRandomAccessStream outputStream = await storageFile.OpenAsync(FileAccessMode.ReadWrite))
            {
                try
                {
                    await RandomAccessStream.CopyAsync(
                        cacheStream.GetInputStreamAt(0L),
                        outputStream.GetOutputStreamAt(0L));
                    return true;
                }
                catch
                {
                    try
                    {
                        // If file was not saved normally, we should delete it
                        await storageFile.DeleteAsync();
                    }
                    catch
                    {
                        LogUtils.Log("[error] can not delete unsaved file: " + fullFilePath);
                    }
                }
            }
            LogUtils.Log("[error] can not save cache to the: " + fullFilePath);
            return false;
        }
        public async virtual Task<IRandomAccessStream> LoadCacheStreamAsync(string cacheKey)
        {
            var fullFilePath = GetFullFilePath(CacheFileNameGenerator.GenerateCacheName(cacheKey));
            try
            {
                var cacheFileMemoryStream = new InMemoryRandomAccessStream();
                var storageFile = await SF.GetFileAsync(fullFilePath);
                using (var cacheFileStream = await storageFile.OpenAsync(FileAccessMode.Read))
                {
                    await RandomAccessStream.CopyAsync(
                        cacheFileStream.GetInputStreamAt(0L),
                        cacheFileMemoryStream.GetOutputStreamAt(0L));
                    return cacheFileMemoryStream;
                }
            }
            catch (Exception ex)
            {
                LogUtils.Log("[error] can not load file stream from: " + fullFilePath + " Exception Msg: " + ex.ToString());
                return null;
            }
        }
        protected virtual string GetFullFilePath(string fileName)
        {
            return Path.Combine(CacheDirectory, fileName);
        }

        public virtual async Task<bool> IsCacheExists(string cacheKey)
        {
            var fullFilePath = GetFullFilePath(CacheFileNameGenerator.GenerateCacheName(cacheKey));
            try
            {
                await SF.GetFileAsync(fullFilePath);
                return true;
            }
            catch
            {
                LogUtils.Log("[error] can not check cache existence, file: " + fullFilePath);
                return false;
            }
        }


        public virtual async Task<bool> IsCacheExistsAndAlive(string cacheKey)
        {
            var fullFilePath = GetFullFilePath(CacheFileNameGenerator.GenerateCacheName(cacheKey));
            try
            {
                var storageFile = await SF.GetFileAsync(fullFilePath);
                return CacheMaxLifetimeInMillis <= 0 ? true :
                    ((DateTime.Now - storageFile.DateCreated.DateTime).TotalMilliseconds < CacheMaxLifetimeInMillis);
            }
            catch
            {
                LogUtils.Log("[error] can not check is cache exists and alive, file: " + fullFilePath);
            }
            return false;
        }

        public virtual async Task Clear()
        {
            await DeleteDirContent(CacheDirectory);
        }

        protected virtual async Task DeleteDirContent(string absoluteDirPath)
        {
            var storageFolder = await SF.GetFolderAsync(absoluteDirPath);
            await DeleteFolderContentsAsync(storageFolder);
        }

        public static async Task DeleteFolderContentsAsync(StorageFolder folder,
            StorageDeleteOption option = StorageDeleteOption.Default)
        {
            try
            {
                // Try to delete all files
                var files = await folder.GetFilesAsync();
                foreach (var file in files)
                {
                    try
                    {
                        await file.DeleteAsync(option);
                    }
                    catch
                    {

                    }
                }
                // Iterate through all subfolders
                var subFolders = await folder.GetFoldersAsync();
                foreach (var subFolder in subFolders)
                {
                    try
                    {
                        // Delete the contents
                        await DeleteFolderContentsAsync(subFolder, option);
                        // Delete the subfolder
                        await subFolder.DeleteAsync(option);
                    }
                    catch
                    {

                    }
                }
            }
            catch
            {

            }
        }
    }
}
