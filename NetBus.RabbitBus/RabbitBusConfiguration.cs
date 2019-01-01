using Microsoft.Extensions.DependencyInjection;
using NetBus.Bus;
using NetBus.RabbitBus.Consumer;
using NetBus.RabbitBus.Publisher;
using NetBus.RabbitBus.Topology;
using RabbitMQ.Client;
using System;

namespace NetBus.RabbitBus
{
    public class RabbitBusConfiguration : IBusConfiguration
    {
        public BusApplication Application { get; set; }
        public TimeSpan WaitTimeout { get; set; } = TimeSpan.FromSeconds(10);

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
