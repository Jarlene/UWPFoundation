using Common.Utils;
using Windows.Security.Cryptography.Core;

namespace Common.Cache.Storage.Generator
{
    class SHA512CacheGenerator : ICacheGenerator
    {
        public string GenerateCacheName(string url)
        {
            return AlgorithmUtils.ComputeHash(url, HashAlgorithmNames.Sha512);
        }
    }
}
