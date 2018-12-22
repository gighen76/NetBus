using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetBus
{
    public interface IServicesConfigurator
    {

        void ConfigureServices(IServiceCollection serviceCollection);

    }
}
