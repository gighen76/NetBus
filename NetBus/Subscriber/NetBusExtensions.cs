using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using System.Linq;

namespace NetBus.Subscriber
{
    public static class NetBusExtensions
    {

        public static async Task<Guid> SubscribeSubscriber<T, R>(this NetBus netBus, IServiceProvider serviceProvider) where T : ISubscriber<R> where R : class
        {
            return await netBus.SubscribeSubscriber<T, R>(() => ActivatorUtilities.CreateInstance<T>(serviceProvider));
        }

        public static async Task<Guid> SubscribeSubscriber<T, R>(this NetBus netBus, Func<T> generator) where T : ISubscriber<R> where R : class
        {
            return await netBus.SubscribeAsync(async (BusEvent<R> busEvent) =>
            {
                BusContext busContext = new BusContext(netBus, busEvent);
                T subscriber = generator();
                await subscriber.Execute(busContext, busEvent.Message);
            });
        }

        public static async Task<IEnumerable<Guid>> SubscribeSubscriber(this NetBus netBus, Type subscriberType, IServiceProvider serviceProvider)
        {
            List<Guid> guids = new List<Guid>();
            var subscriberInterfaces = subscriberType
                .GetInterfaces()
                .Where(i => i.GetGenericTypeDefinition() == typeof(ISubscriber<>));
            foreach (var subscriberInterface in subscriberInterfaces)
            {
                var messageType = subscriberInterface.GenericTypeArguments[0];

                var method = typeof(NetBusExtensions).GetMethod("SubscribeSubscriber", new Type[] { typeof(NetBus), typeof(IServiceProvider) });

                guids.Add(await (Task<Guid>)method
                    .MakeGenericMethod(new Type[] { subscriberType, messageType })
                    .Invoke(null, new object[] { netBus, serviceProvider }));

            }
            return guids;
        }

        public static async Task<IEnumerable<Guid>> SubscribeSubscriber(this NetBus netBus, Assembly assembly, IServiceProvider serviceProvider)
        {
            List<Guid> guids = new List<Guid>();
            var subscriberTypes = assembly.ExportedTypes
                .Where(t => t.GetInterfaces()
                    .Any(i => i.GetGenericTypeDefinition() == typeof(ISubscriber<>)));
            foreach (var subscriberType in subscriberTypes)
            {
                guids.AddRange(await SubscribeSubscriber(netBus, subscriberType, serviceProvider));
            }
            return guids;
        }

    }
}