using System;
using System.Collections;
using System.Collections.Generic;

namespace Common.Cache.Memory
{
    public class SynchronizedWeakRefDictionary<TKey, TValue> : IDictionary<TKey, TValue> where TKey : class where TValue : class
    {

        private readonly WeakRefDictionary<TKey, TValue> weakRefDictionary = new WeakRefDictionary<TKey, TValue>();

        private readonly object lockObj = new object();

        private readonly IList<TKey> keyList = new List<TKey>();

        public TValue this[TKey key]
        {
            get
            {
                lock (lockObj)
                {
                    TValue value;
                    return weakRefDictionary.TryGetValue(key, out value) ? value : null;
                }
            }

            set
            {
                lock (lockObj)
                {
                    weakRefDictionary.Remove(key);
                    weakRefDictionary.Add(key, value);
                }
            }
        }

        public int Count
        {
            get;
            private set;
        }

        public bool IsReadOnly
        {
            get;
            private set;
        }

        public ICollection<TKey> Keys
        {
            get
            {
                lock (lockObj)
                {
                    return new List<TKey>(keyList);
                }
            }
        }

        public ICollection<TValue> Values
        {
            get;
            private set;
        }

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            lock (lockObj)
            {
                weakRefDictionary.Add(item.Key, item.Value);
                keyList.Add(item.Key);
                Count++;
            }
        }

        public void Add(TKey key, TValue value)
        {
            lock (lockObj)
            {
                weakRefDictionary.Add(key, value);
                keyList.Add(key);
                Count++;
            }
        }

        public void Clear()
        {
            lock (lockObj)
            {
                foreach (var key in keyList)
                {
                    weakRefDictionary.Remove(key);
                }
                keyList.Clear();
                Count = 0;
            }
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            lock (lockObj)
            {
                TValue o;
                return weakRefDictionary.TryGetValue(item.Key, out o);
            }
        }

        public bool ContainsKey(TKey key)
        {
            lock (lockObj)
            {
                return weakRefDictionary.ContainsKey(key);
            }
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return weakRefDictionary.GetEnumerator();
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            throw new NotImplementedException();
        }

        public bool Remove(TKey key)
        {
            lock (lockObj)
            {
                if (keyList.Remove(key))
                {
                    Count--;
                    weakRefDictionary.Remove(key);
                    return true;
                }

                return false;
            }
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            lock (lockObj)
            {
                return weakRefDictionary.TryGetValue(key, out value);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
