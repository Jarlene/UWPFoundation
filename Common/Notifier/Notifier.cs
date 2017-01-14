﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Windows.UI.Core;

namespace Common.Notifier
{
    public class Notifier
    {
        public readonly static Notifier Default = new Notifier();

        public static CoreDispatcher MainDispatcher { get; set; }

        static SynchronizationContext _syncContext;

        private readonly object _lockForSubscribersByType = new object();

        private readonly ConcurrentDictionary<Type, object> _locksForSubscription = new ConcurrentDictionary<Type, object>();

        private readonly object _lockForSubscribersList = new object();

        private ConcurrentDictionary<Type, List<Subscription>> _subscriptionDictByType = new ConcurrentDictionary<Type, List<Subscription>>();

        private ConditionalWeakTable<object, List<Type>> _subscriberDictWithType = new ConditionalWeakTable<object, List<Type>>();

        public void Register(object subscriber)
        {
            if (_syncContext == null && SynchronizationContext.Current != null)
                _syncContext = SynchronizationContext.Current;

            List<Type> subscriptTypes = new List<Type>();
            if (_subscriberDictWithType.ContainsKey(subscriber))
            {
                return;
            }

            IList<Subscription> subscriptionList = SubscriptionHandler.CreateSubscription(subscriber);

            foreach (var subscription in subscriptionList)
            {
                var subscriptionsOfType = _subscriptionDictByType.GetOrAdd(subscription.EventType, new List<Subscription>());

                lock (_locksForSubscription.GetOrAdd(subscription.EventType, new object()))
                {
                    subscriptionsOfType.Add(subscription);
                    subscriptionsOfType.Sort();
                }

                if (!subscriptTypes.Contains(subscription.EventType))
                {
                    subscriptTypes.Add(subscription.EventType);
                }
            }

            _subscriberDictWithType.Add(subscriber, subscriptTypes);
        }



        public void Unregister(object subscriber)
        {
            List<Type> types = null;
            if (_subscriberDictWithType.TryGetValue(subscriber, out types))
            {
                foreach (var type in types)
                {
                    RemoveSubscription(type, subscriber);
                }
            }
        }

        void RemoveSubscription(Type eventType, object subscriber)
        {
            List<Subscription> subscriptions;
            if (_subscriptionDictByType.TryGetValue(eventType, out subscriptions))
            {
                lock (_locksForSubscription.GetOrAdd(eventType, new object()))
                {
                    subscriptions.RemoveAll(o => o.Subscriber == subscriber);
                }
            }
        }

        public void Notify(object eventObj)
        {
            if (!_subscriptionDictByType.ContainsKey(eventObj.GetType()))
                return;

            var subscriptionsOfType = _subscriptionDictByType[eventObj.GetType()];
            List<Subscription> subList;
            lock (_locksForSubscription.GetOrAdd(eventObj.GetType(), new object()))
            {
                subList = subscriptionsOfType.ToList();
            }

            foreach (var subscription in subList)
            {
                if (subscription.IsSubscriberAlive && _subscriberDictWithType.ContainsKey(subscription.Subscriber))
                {
                    DispatchNotification(subscription, eventObj);
                }
            }
        }

        void DispatchNotification(Subscription subscription, object eventObj)
        {
            switch (subscription.ThreadMode)
            {
                case ThreadMode.Current:
                    subscription.ExecCallback(eventObj);
                    return;
                case ThreadMode.Background:
                    Task.Run(() => subscription.ExecCallback(eventObj));
                    return;
                case ThreadMode.Main:
                    if (SynchronizationContext.Current == null)
                    {
                        if (MainDispatcher != null)
                            MainDispatcher.RunAsync(CoreDispatcherPriority.Normal, () => subscription.ExecCallback(eventObj)).AsTask();
                        else if (_syncContext != null)
                            _syncContext.Post((a) => subscription.ExecCallback(eventObj), null);
                        else
                            throw new Exception("cannot get ui dispatcher");
                    }
                    else
                    {
                        subscription.ExecCallback(eventObj);
                    }
                    return;
                default:
                    throw new Exception(subscription.ThreadMode.ToString() + " is not supported");
            }
        }
    }
}
