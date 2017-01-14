using Common.Utils;
using Windows.Security.Cryptography.Core;

namespace Common.Cache.Storage.Generator
{
    public class SHA384CacheGenerator : ICacheGenerator
    {
        public string GenerateCacheName(string url)
        {
            return AlgorithmUtils.ComputeHash(url, HashAlgorithmNames.Sha384);
        }
    }
}
