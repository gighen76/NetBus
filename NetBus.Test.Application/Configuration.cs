using System.Threading.Tasks;
using NetBus.Host;
using NetBus.Test.Application.Messages;

namespace NetBus.Test.Application
{
    class Configuration : INetBusServiceConfiguration
    {

        private readonly NetBus netBus;

        public Configuration(NetBus netBus)
        {
            this.netBus = netBus;
        }

        public async Task OnStarted()
        {
            
            await netBus.PublishAsync<CreateOrder>(new CreateOrder
            {
                Id = 1
            });

        }
    }
}