using System.Collections;
using System.Collections.Generic;

namespace Common.Cache.Storage
{
    public class SynchronizedDictionary<TKey, TValue> : IDictionary<TKey, TValue>
    {

        private readonly IDictionary<TKey, TValue> dictionary = new Dictionary<TKey, TValue>();
        private readonly object lockObj = new object();
        public TValue this[TKey key]
        {
            get
            {
                lock (lockObj)
                {
                    return dictionary[key];
                }
            }

            set
            {
                lock (lockObj)
                {
                    dictionary[key] = value;
                }
            }
        }

        public int Count
        {
            get
            {
                lock (lockObj)
                {
                    return dictionary.Count;
                }
            }
        }

        public bool IsReadOnly
        {
            get
            {
                lock (lockObj)
                {
                    return dictionary.IsReadOnly;
                }
            }
        }

        public ICollection<TKey> Keys
        {
            get
            {
                lock (lockObj)
                {
                    return dictionary.Keys;
                }
            }
        }

        public ICollection<TValue> Values
        {
            get
            {
                lock (lockObj)
                {
                    return dictionary.Values;
                }
            }
        }

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            lock (lockObj)
            {
                dictionary.Add(item);
            }
        }

        public void Add(TKey key, TValue value)
        {
            lock (lockObj)
            {
                dictionary.Add(key, value);
            }
        }

        public void Clear()
        {
            lock (lockObj)
            {
                dictionary.Clear();
            }
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            lock (lockObj)
            {
                return dictionary.Contains(item);
            }
        }

        public bool ContainsKey(TKey key)
        {
            lock (lockObj)
            {
                return dictionary.ContainsKey(key);
            }
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            lock (lockObj)
            {
                dictionary.CopyTo(array, arrayIndex);
            }
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            lock (lockObj)
            {
                return dictionary.GetEnumerator();
            }
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            lock (lockObj)
            {
                return dictionary.Remove(item);
            }
        }

        public bool Remove(TKey key)
        {
            lock (lockObj)
            {
                return dictionary.Remove(key);
            }
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            lock (lockObj)
            {
                return dictionary.TryGetValue(key, out value);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
