using System;
using System.Threading.Tasks;

namespace NetBus.Bus
{
    public abstract class BaseBus
    {

        private readonly IBusConfiguration busConfiguration;

        public BaseBus(IBusConfiguration busConfiguration)
        {
            this.busConfiguration = busConfiguration ?? throw new ArgumentNullException(nameof(busConfiguration));
        }

        public string SubscriberName => busConfiguration.SubscriberName;

        private readonly object m_eventLock = new object();
        private Func<string, byte[], Task> _OnMessage;
        public event Func<string, byte[], Task> OnMessage
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

        protected Task ProcessMessage(string topicName, byte[] message)
        {
            return _OnMessage(topicName, message);
        }

        abstract public Task PublishAsync(string topicName, byte[] message);
        
        abstract public Task SubscribeAsync(string topicName);

    }
}
