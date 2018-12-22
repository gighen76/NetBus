using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NetBus.Subscriber;
using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace NetBus.Host
{
    class NetBusService<T> : IHostedService where T : INetBusServiceConfiguration
    {

        private readonly NetBus netBus;
        private readonly IServiceProvider serviceProvider;
        private readonly INetBusServiceConfiguration configuration;

        public NetBusService(NetBus netBus, IServiceProvider serviceProvider)
        {
            this.netBus = netBus;
            this.serviceProvider = serviceProvider;

            this.configuration = ActivatorUtilities.CreateInstance<T>(serviceProvider);

        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await netBus.SubscribeSubscriber(Assembly.GetEntryAssembly(), serviceProvider);
            await configuration.OnStarted();
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            
        }

    }
}