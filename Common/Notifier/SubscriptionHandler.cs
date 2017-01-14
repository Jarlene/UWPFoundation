using System.Collections.Generic;
using System.Reflection;

namespace Common.Notifier
{
    public class SubscriptionHandler
    {
        public static IList<Subscription> CreateSubscription(object subscriber)
        {
            IList<Subscription> subscriptionList = new List<Subscription>();

            var methodInfos = subscriber.GetType().GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static);
            foreach (var methodInfo in methodInfos)
            {
                var paramsInfo = methodInfo.GetParameters();
                var returnType = methodInfo.ReturnType;

                if (!methodInfo.IsDefined(typeof(SubscriberCallbackAttribute)) || paramsInfo.Length > 1 || returnType != typeof(void))
                    continue;

                var attr = methodInfo.GetCustomAttribute<SubscriberCallbackAttribute>(true);

                subscriptionList.Add(new Subscription(subscriber, methodInfo, attr.EventType, attr.Priority, attr.ThreadMode));
            }

            return subscriptionList;
        }
    }
}
