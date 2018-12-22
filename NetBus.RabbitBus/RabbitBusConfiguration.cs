using Microsoft.Extensions.DependencyInjection;
using NetBus.Bus;
using NetBus.RabbitBus.Topology;
using NetBus.RabbitBus.Publisher;
using RabbitMQ.Client;
using System;
using NetBus.RabbitBus.Consumer;

namespace NetBus.RabbitBus
{
    public class RabbitBusConfiguration : IBusConfiguration
    {
        public string SubscriberName { get; set; }
        public string TracerName { get; set; }

        public string Uri { get; set; }
        public TimeSpan RecoveryInterval { get; set; }
        public ushort PrefetchCount { get; set; }

        public void ConfigureServices(IServiceCollection serviceCollection)
        {

            serviceCollection.AddSingleton<IConnectionFactory>(sp => new ConnectionFactory
            {
                Uri = new Uri(Uri),
                AutomaticRecoveryEnabled = true,
                TopologyRecoveryEnabled = false,
                NetworkRecoveryInterval = RecoveryInterval,
                DispatchConsumersAsync = true
            });

            serviceCollection.AddSingleton<ITopology, DefaultTopology>();
            serviceCollection.AddSingleton<IPublisher, DefaultPublisher>();
            serviceCollection.AddSingleton<IConsumer, DefaultConsumer>();
            serviceCollection.AddSingleton<BaseBus, RabbitBus>();

        }
    }
}
