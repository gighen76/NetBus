using Microsoft.Extensions.DependencyInjection;
using NetBus.Bus;

namespace NetBus.MockBus
{
    public class MockBusConfiguration : IBusConfiguration
    {
        public BusApplication Application { get; set; }

        public void ConfigureServices(IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<BaseBus, MockBus>();
        }
    }
}