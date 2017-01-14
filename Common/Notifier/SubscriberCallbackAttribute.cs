using System;

namespace Common.Notifier
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class SubscriberCallbackAttribute : Attribute
    {
        const NotifyPriority DEFAULT_PRIORITY = NotifyPriority.Normal;
        const ThreadMode DEFAULT_THREAD_MODE = ThreadMode.Current;

        public SubscriberCallbackAttribute(Type eventType,
            NotifyPriority priority = DEFAULT_PRIORITY, ThreadMode threadMode = DEFAULT_THREAD_MODE)
        {
            EventType = eventType;
            Priority = priority;
            ThreadMode = threadMode;
        }

        public Type EventType { get; set; }

        public NotifyPriority Priority { get; set; }

        public ThreadMode ThreadMode { get; set; }
    }
}
