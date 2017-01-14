using System.Runtime.CompilerServices;

namespace Common.Notifier
{
    public static class Extersion
    {
        public static bool ContainsKey<T1, T2>(this ConditionalWeakTable<T1, T2> weakTable, T1 key) where T1 : class where T2 : class
        {
            T2 value;
            return weakTable.TryGetValue(key, out value);
        }
    }
}
