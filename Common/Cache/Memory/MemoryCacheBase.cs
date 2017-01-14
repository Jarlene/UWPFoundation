using System;

namespace Common.Cache.Memory
{
    public abstract class MemoryCacheBase<TKey, TValue> where TKey : class where TValue : class
    {
        public abstract TValue Get(TKey k);

        public abstract bool TryGetValue(TKey k, out TValue v);

        public abstract void Put(TKey k, TValue v, long exp = 0);

        public abstract void Clear();

        protected class CacheItem<K, V>
        {
            public CacheItem(K k, V v, long exp = 0)
            {
                Key = k;
                Value = v;
                if (exp == 0)
                {
                    Exp = 0;
                }
                else
                {
                    Exp = DateTime.Now.Millisecond + exp;
                }

            }
            public K Key
            {
                get;
                private set;
            }
            public V Value
            {
                get;
                private set;
            }

            public long Exp
            {
                get;
                private set;
            }
        }
    }


}
