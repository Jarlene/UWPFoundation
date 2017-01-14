
using System;

namespace Common.Cache.Memory.Impl
{
    public class WeakMemoryCache<TKey, TValue> : MemoryCacheBase<TKey, TValue> where TKey : class where TValue : class
    {

        private readonly SynchronizedWeakRefDictionary<TKey, CacheItem<TKey, TValue>> synchronizedWeakDictionary = new SynchronizedWeakRefDictionary<TKey, CacheItem<TKey, TValue>>();
        public override void Clear()
        {
            synchronizedWeakDictionary.Clear();
        }

        public override TValue Get(TKey k)
        {
            CacheItem<TKey, TValue> node = synchronizedWeakDictionary[k];
            if (node.Exp <= 0 || node.Exp >= DateTime.Now.Millisecond)
            {
                return node.Value;
            } else
            {
                synchronizedWeakDictionary.Remove(k);
                return default(TValue);
            }
               
        }

        public override void Put(TKey k, TValue v, long exp = 0)
        {
            CacheItem<TKey, TValue> node = new CacheItem<TKey, TValue>(k, v, exp);
            synchronizedWeakDictionary[k] = node;
        }

        public override bool TryGetValue(TKey k, out TValue v)
        {
            CacheItem<TKey, TValue> node = synchronizedWeakDictionary[k];
            if (node.Exp <= 0 || node.Exp >= DateTime.Now.Millisecond)
            {
                v = node.Value;
                return true;
            }
            else
            {
                synchronizedWeakDictionary.Remove(k);
                v = default(TValue);
                return false;
            }
        }
    }
}
