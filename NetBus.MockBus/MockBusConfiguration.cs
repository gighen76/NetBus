using Microsoft.Extensions.DependencyInjection;
using NetBus.Bus;

namespace NetBus.MockBus
{
    public class MockBusConfiguration : IBusConfiguration
    {
        public string SubscriberName { get; set; }

        public void ConfigureServices(IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<BaseBus, MockBus>();
        }
    }
}