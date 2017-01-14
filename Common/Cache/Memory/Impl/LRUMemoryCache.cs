using System;
using System.Collections.Generic;
using Windows.Storage.Streams;

namespace Common.Cache.Memory.Impl
{
    public class LRUMemoryCache : LRUCache<String, IRandomAccessStream>
    {
        private int currentSize;

        public LRUMemoryCache(int capacity = 1024 * 1024 * 1024)
        {
            this.capacity = capacity;
        }

        protected override bool checkSize(string key, IRandomAccessStream value)
        {
            var size = (int)value.Size;
            currentSize += size;
            while (currentSize > capacity && lruList.Count > 0)
            {
                this.removeFirst();
            }
            return true;
        }

        protected override void removeNode(LinkedListNode<CacheItem<string, IRandomAccessStream>> node)
        {
            base.removeNode(node);
            currentSize -= (int)node.Value.Value.Size;
        }
    }
}
