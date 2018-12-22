using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetBus;
using NetBus.Bus;

namespace NetBus.Host
{


    public static class NetBusHostExtensions
    {

        public static IHostBuilder UseNetBusService<T,C>(this IHostBuilder hostBuilder, Action<T> configureConfiguration) where T : class, IBusConfiguration where C: INetBusServiceConfiguration
        {

            hostBuilder.ConfigureServices(serviceCollection =>
            {
                serviceCollection.UseNetBus<T>(configureConfiguration);
                serviceCollection.AddHostedService<NetBusService<C>>();
            });

            return hostBuilder;

        }




    }
}