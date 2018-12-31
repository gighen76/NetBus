using Microsoft.Extensions.DependencyInjection;

namespace NetBus
{
    public interface IServicesConfigurator
    {

        void ConfigureServices(IServiceCollection serviceCollection);

    }
}
