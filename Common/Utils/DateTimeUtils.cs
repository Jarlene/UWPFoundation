using System;

namespace Common.Utils
{
    public static class DateTimeUtils
    {
        private static readonly DateTime ZeroDateTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        /// <summary>
        /// Fully Java analog for System.currentTimeMillis()
        /// </summary>
        /// <returns></returns>
        public static long CurrentTimeMillis()
        {
            return DateTime.Now.Millisecond;
        }

        public static long Milliseconds(this DateTime dateTime)
        {
            return (long)(dateTime - ZeroDateTime).TotalMilliseconds;
        }
    }
}
