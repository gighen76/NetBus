using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        private Func<BusTopic, BusEvent, byte[], Task> _OnMessage;
        public event Func<BusTopic, BusEvent, byte[], Task> OnMessage
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

        protected async Task ProcessMessage(byte[] message, IDictionary<string, string> headers)
        {
            if (headers.ContainsKey("TopicName") && BusTopic.TryParse(headers["TopicName"], out BusTopic topic) &&
                headers.ContainsKey("Id") && Guid.TryParse(headers["Id"], out Guid id) &&
                headers.ContainsKey("ParentId") && Guid.TryParse(headers["ParentId"], out Guid parentId) &&
                headers.ContainsKey("OriginId") && Guid.TryParse(headers["OriginId"], out Guid originId))
            {

                var busEvent = new BusEvent
                {
                    Id = id,
                    ParentId = parentId,
                    OriginId = originId
                };

                await _OnMessage(topic, busEvent, message);
            }
        }

        public async Task PublishAsync(BusTopic topic, byte[] message, BusEvent parentEvent = null)
        {
            Guid eventId = Guid.NewGuid();

            var headers = new Dictionary<string, string>
            {
                { "TopicName", topic.Name },
                { "Id", eventId.ToString() },
                { "ParentId", parentEvent?.Id.ToString() ?? eventId.ToString() },
                { "OriginId", parentEvent?.OriginId.ToString() ?? eventId.ToString() }
            };

            await ConcretePublishAsync(topic, message, headers);

            headers.Add("TraceType", "PUBLISH");
            await ConcretePublishAsync(Configuration.TracerTopic, message, headers);
        }

        public async Task SubscribeAsync(BusTopic topic)
        {
            await ConcreteSubscribeAsync(topic);
        }

        abstract protected Task ConcretePublishAsync(BusTopic topic, byte[] message, IDictionary<string, string> headers);
        
        abstract protected Task ConcreteSubscribeAsync(BusTopic topic);



    }
}
