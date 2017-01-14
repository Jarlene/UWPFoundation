using System;
using System.Collections.Generic;

namespace Common.Cache.Memory
{
    public class LRUCache<TKey, TValue> : MemoryCacheBase<TKey, TValue> where TKey : class where TValue : class
    {
        private readonly object lockUp = new object();

        private Dictionary<TKey, LinkedListNode<CacheItem<TKey, TValue>>> cacheMap
            = new Dictionary<TKey, LinkedListNode<CacheItem<TKey, TValue>>>();


        protected LinkedList<CacheItem<TKey, TValue>> lruList = new LinkedList<CacheItem<TKey, TValue>>();

        protected int capacity;


        public LRUCache(int cap = 20)
        {
            this.capacity = cap;
        }


        public override void Clear()
        {
            lruList.Clear();
            cacheMap.Clear();
        }

        public override TValue Get(TKey k)
        {
            lock (lockUp)
            {
                LinkedListNode<CacheItem<TKey, TValue>> node;
                if (cacheMap.TryGetValue(k, out node))
                {
                    if (node.Value.Exp <= 0 || node.Value.Exp >= DateTime.Now.Millisecond) // 没有过期
                    {
                        TValue value = node.Value.Value;
                        lruList.Remove(node);
                        lruList.AddLast(node);
                        return value;
                    } else
                    {
                        lruList.Remove(node);
                    }

                }
                return default(TValue);
            }
        }

        public override void Put(TKey k, TValue v, long exp = 0)
        {
            lock (lockUp)
            {
                if (cacheMap.ContainsKey(k))
                {
                    LinkedListNode<CacheItem<TKey, TValue>> tempNode;
                    cacheMap.TryGetValue(k, out tempNode);
                    cacheMap.Remove(k);
                    lruList.Remove(tempNode);
                }
                this.checkSize(k, v);
                CacheItem<TKey, TValue> cacheItem = new CacheItem<TKey, TValue>(k, v, exp);
                LinkedListNode<CacheItem<TKey, TValue>> node =
                    new LinkedListNode<CacheItem<TKey, TValue>>(cacheItem);
                lruList.AddLast(node);
                cacheMap.Add(k, node);
            }
        }

        public override bool TryGetValue(TKey k, out TValue v)
        {
            lock (lockUp)
            {
                LinkedListNode<CacheItem<TKey, TValue>> node;
                if (cacheMap.TryGetValue(k, out node))
                {
                    if (node.Value.Exp <= 0 || node.Value.Exp >= DateTime.Now.Millisecond) // 没有过期
                    {
                        v = node.Value.Value;
                        lruList.Remove(node);
                        lruList.AddLast(node);
                        return true;
                    } else // 过期了
                    {
                        lruList.Remove(node);
                    }
                    
                }
                v = default(TValue);
                return false;
            }
        }


        protected virtual bool checkSize(TKey key, TValue value)
        {
            if (cacheMap.Count >= capacity)
            {
                removeFirst();
            }
            return true;
        }

        protected void removeFirst()
        {
            LinkedListNode< CacheItem<TKey, TValue>> node = lruList.First;
            this.removeNode(node);
        }

        protected virtual void removeNode(LinkedListNode<CacheItem<TKey, TValue>> node)
        {
            lruList.Remove(node);
            cacheMap.Remove(node.Value.Key);
        }

        
    }
}
