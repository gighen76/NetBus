using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using NetBus.Bus;
using NetBus.Serializer;
using NetBus.TopicResolver;
using NetBus.Tracer;
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



    }
}