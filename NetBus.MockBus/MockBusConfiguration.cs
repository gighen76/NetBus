using Microsoft.Extensions.DependencyInjection;
using NetBus.Bus;
using System;

namespace NetBus.MockBus
{
    public class MockBusConfiguration : IBusConfiguration
    {
        public BusApplication Application { get; set; }
        public BusTopic TracerTopic { get; set; }
        public TimeSpan WaitTimeout { get; set; } = TimeSpan.FromSeconds(10);

        public void ConfigureServices(IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<BaseBus, MockBus>();
        }
    }
}