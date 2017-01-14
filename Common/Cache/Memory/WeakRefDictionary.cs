using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Common.Cache.Memory
{
    public class WeakRefDictionary<TKey, TValue>
    {
        readonly Dictionary<TKey, WeakReference> inner = new Dictionary<TKey, WeakReference>();

        public int Count
        {
            get
            {
                CleanAbandonedItems();
                return inner.Count;
            }
        }

        public TValue this[TKey key]
        {
            get
            {
                TValue result;

                if (TryGetValue(key, out result))
                    return result;

                throw new KeyNotFoundException();
            }
        }

        public void Add(TKey key,
                        TValue value)
        {
            TValue dummy;

            if (TryGetValue(key, out dummy))
                throw new ArgumentException("KeyAlreadyPresent");

            inner.Add(key, new WeakReference(EncodeNullObject(value)));
        }

        void CleanAbandonedItems()
        {
            List<TKey> deadKeys = new List<TKey>();

            foreach (KeyValuePair<TKey, WeakReference> kvp in inner)
                if (kvp.Value.Target == null)
                    deadKeys.Add(kvp.Key);

            foreach (TKey key in deadKeys)
                inner.Remove(key);
        }

        public bool ContainsKey(TKey key)
        {
            TValue dummy;
            return TryGetValue(key, out dummy);
        }

        static TObject DecodeNullObject<TObject>(object innerValue)
        {
            if (innerValue == typeof(NullObject))
                return default(TObject);
            else
                return (TObject)innerValue;
        }

        static object EncodeNullObject(object value)
        {
            if (value == null)
                return typeof(NullObject);
            else
                return value;
        }

        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures",
           Justification = "Standard interface for dictionary like types.")]
        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            foreach (KeyValuePair<TKey, WeakReference> kvp in inner)
            {
                object innerValue = kvp.Value.Target;

                if (innerValue != null)
                    yield return new KeyValuePair<TKey, TValue>(kvp.Key, DecodeNullObject<TValue>(innerValue));
            }
        }

        public bool Remove(TKey key)
        {
            return inner.Remove(key);
        }

        public bool TryGetValue(TKey key,
                           out TValue value)
        {
            value = default(TValue);
            WeakReference wr;

            if (!inner.TryGetValue(key, out wr))
                return false;

            object result = wr.Target;

            if (result == null)
            {
                inner.Remove(key);
                return false;
            }

            value = DecodeNullObject<TValue>(result);
            return true;
        }

        [SuppressMessage("Microsoft.Performance", "CA1812:AvoidUninstantiatedInternalClasses",
            Justification = "Type object is used as a marker")]
        class NullObject { }
    }
}
