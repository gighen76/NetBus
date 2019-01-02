using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace NetBus.Bus
{
    public abstract class BaseBus
    {

        private readonly ConcurrentDictionary<BusTopic, ConcurrentDictionary<Guid,Action<BusEvent>>> waitForActions
            = new ConcurrentDictionary<BusTopic, ConcurrentDictionary<Guid, Action<BusEvent>>>();

        public BaseBus(IBusConfiguration busConfiguration)
        {
            Configuration = busConfiguration ?? throw new ArgumentNullException(nameof(busConfiguration));
        }

        public IBusConfiguration Configuration { get; }

        private readonly object m_eventLock = new object();
        private Func<BusEvent, Task> _OnMessage;
        public event Func<BusEvent, Task> OnMessage
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

        protected async Task ProcessMessage(byte[] message, IDictionary<string, string> busHeaders)
        {
            var busEvent = new BusEvent(message, busHeaders);

            if (waitForActions.TryGetValue(busEvent.Topic, out ConcurrentDictionary<Guid, Action<BusEvent>> waitForTopicActions))
            {
                if (waitForTopicActions.TryGetValue(busEvent.OriginId, out Action<BusEvent> action))
                {
                    action(busEvent);
                }
            }

            await _OnMessage(busEvent);

        }

        public async Task PublishAsync(BusEvent busEvent)
        {
            await ConcretePublishAsync(busEvent.Topic, busEvent.Message, busEvent.GetBusHeaders());
        }

        public async Task SubscribeAsync(BusTopic topic)
        {
            await ConcreteSubscribeAsync(topic);
        }

        public async Task<BusEvent> PublishAndWaitAsync(BusEvent busEvent, BusTopic waitForTopic)
        {
            TaskCompletionSource<BusEvent> tcs = new TaskCompletionSource<BusEvent>();
            Timer timer = null;

            var waitForTopicActions = waitForActions.GetOrAdd(waitForTopic, new ConcurrentDictionary<Guid, Action<BusEvent>>());
            waitForTopicActions.GetOrAdd(busEvent.OriginId, waitedEventBus =>
            {
                timer?.Dispose();
                waitForTopicActions.TryRemove(busEvent.OriginId, out Action<BusEvent> removed);
                tcs.SetResult(waitedEventBus);
            });
            await SubscribeAsync(waitForTopic);

            timer = new Timer(state =>
            {
                timer?.Dispose();
                waitForTopicActions.TryRemove(busEvent.OriginId, out Action<BusEvent> removed);
                tcs.TrySetException(new TimeoutException($"Request timed out."));
            }, null, Configuration.WaitTimeout, TimeSpan.FromMilliseconds(-1));

            await PublishAsync(busEvent);

            return await tcs.Task;
        }

        abstract protected Task ConcretePublishAsync(BusTopic topic, byte[] message, IDictionary<string, string> headers);
        
        abstract protected Task ConcreteSubscribeAsync(BusTopic topic);



    }
}
