using System;
using System.Threading.Tasks;

namespace NetBus.Bus
{
    public abstract class BaseBus
    {

        public BaseBus(IBusConfiguration busConfiguration)
        {
            Configuration = busConfiguration ?? throw new ArgumentNullException(nameof(busConfiguration));
        }

        public IBusConfiguration Configuration { get; }

        private readonly object m_eventLock = new object();
        private Func<BusTopic, byte[], Task> _OnMessage;
        public event Func<BusTopic, byte[], Task> OnMessage
        {
            add
            {
                lock (m_eventLock)
                {
                    _OnMessage += value;
                }
            }
            remove
            {
                lock (m_eventLock)
                {
                    _OnMessage -= value;
                }
            }
        }

        protected Task ProcessMessage(BusTopic topic, byte[] message)
        {
            return _OnMessage(topic, message);
        }

        abstract public Task PublishAsync(BusTopic topic, byte[] message);
        
        abstract public Task SubscribeAsync(BusTopic topic);



    }
}
