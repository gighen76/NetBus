using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using NetBus.Bus;
using NetBus.Serializer;
using NetBus.TopicResolver;
using System;

namespace NetBus
{
    static public class NetBusExtensions
    {

        static public IServiceCollection UseNetBus<TBusConfiguration>(this IServiceCollection serviceCollection, Action<TBusConfiguration> configureBusConfiguration) where TBusConfiguration : class, IBusConfiguration
        {

            var configuration = Activator.CreateInstance<TBusConfiguration>();
            configureBusConfiguration(configuration);
            configuration.ConfigureServices(serviceCollection);
            serviceCollection.AddSingleton(configuration);

            serviceCollection.TryAddSingleton<ISerializer, DefaultSerializer>();
            serviceCollection.TryAddSingleton<ITopicResolver, DefaultTopicResolver>();
            serviceCollection.TryAddSingleton<NetBus>();

            return serviceCollection;

        }

        static public void LogPublish<T>(this ILogger logger, BusApplication application, BusTopic topic, BusEvent<T> busEvent) where T: class
        {
            if (logger != null)
            {
                logger.Log(LogLevel.Information, new EventId(1, "PUBLISH"), new
                {
                    Application = application,
                    Topic = topic,
                    BusEvent = busEvent
                }, null, (o, ex) => $"{o.Application} -> {o.Topic}");
            }

        }

        static public void LogConsume(this ILogger logger, BusApplication application, BusTopic topic, BusEvent busEvent)
        {
            if (logger != null)
            {
                logger.Log(LogLevel.Information, new EventId(2, "CONSUME"), new
                {
                    Application = application,
                    Topic = topic,
                    BusEvent = busEvent
                }, null, (o, ex) => $"{o.Application} -> {o.Topic}");
            }
        }


    }
}