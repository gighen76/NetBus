using System.Threading.Tasks;

namespace NetBus.Subscriber
{
    public class BusContext
    {

        private readonly NetBus netBus;
        private readonly BusEvent busEvent;

        public BusContext(NetBus netBus, BusEvent busEvent)
        {
            this.netBus = netBus;
            this.busEvent = busEvent;
        }

        public NetBus NetBus { get; }
        public BusEvent BusEvent { get; }

        public Task PublishAsync<T>(T message) where T: class
        {
            return netBus.PublishAsync(message, busEvent);
        }



    }
}