namespace Common.Cache.Storage.Generator
{
    public interface ICacheGenerator
    {
        string GenerateCacheName(string url);
    }
}
