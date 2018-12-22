using Microsoft.Extensions.DependencyInjection;

namespace NetBus.Bus
{
    public interface IBusConfiguration
    {

        string SubscriberName { get; set; }
        string TracerName { get; set; }

        void ConfigureServices(IServiceCollection serviceCollection);

    }
}