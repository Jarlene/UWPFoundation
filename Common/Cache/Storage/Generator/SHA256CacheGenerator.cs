using Common.Utils;
using Windows.Security.Cryptography.Core;

namespace Common.Cache.Storage.Generator
{
    public class SHA256CacheGenerator : ICacheGenerator
    {
        public string GenerateCacheName(string url)
        {
            return AlgorithmUtils.ComputeHash(url, HashAlgorithmNames.Sha256);
        }
    }
}
