using NetBus.Bus;
using NetBus.Serializer;
using NetBus.TopicResolver;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace NetBus
{
    public class NetBus
    {

        private ConcurrentDictionary<BusTopic, ConcurrentDictionary<Guid,Func<BusEvent,Task>>> topicHandlers = 
            new ConcurrentDictionary<BusTopic, ConcurrentDictionary<Guid, Func<BusEvent, Task>>>();

        private readonly BaseBus bus;
        private readonly ISerializer serializer;
        private readonly ITopicResolver topicResolver;

        public NetBus(BaseBus bus, ISerializer serializer, 
            ITopicResolver topicResolver)
        {
            
            this.bus = bus;
            this.bus.OnMessage += Bus_OnMessage;
            this.serializer = serializer;
            this.topicResolver = topicResolver;
        }

        private async Task Bus_OnMessage(BusEvent busEvent)
        {
            if (topicHandlers.ContainsKey(busEvent.Topic))
            {
                foreach (var handler in topicHandlers[busEvent.Topic])
                {
                    await handler.Value(busEvent);
                }
            }
        }

        public async Task PublishAsync<T>(T message, BusEvent parent = null) where T : class
        {
            var messageBytes = serializer.Serialize(message);

            var topic = topicResolver.ResolveTopicName<T>();

            await bus.PublishAsync(topic, messageBytes, parent);

        }

        public async Task<Guid> SubscribeAsync<T>(Func<BusEvent, T, Task> handler) where T : class
        {
            var topic = topicResolver.ResolveTopicName<T>();

            var topicHandler = topicHandlers.GetOrAdd(topic,
                new ConcurrentDictionary<Guid, Func<BusEvent, Task>>());

            Guid handlerGuid = Guid.NewGuid();
            topicHandler.GetOrAdd(handlerGuid, async (busEvent) =>
            {
                var message = serializer.Deserialize<T>(busEvent.Message);
                await handler(busEvent, message);
            });

            await bus.SubscribeAsync(topic);
            return handlerGuid;
        }

        public async Task<R> PublishAndWaitAsync<T, R>(T message, TimeSpan timeout, BusEvent parent = null) where T : class where R : class
        {
            parent = parent ?? new BusEvent
            {
                Id = Guid.NewGuid(),
                ParentId = Guid.NewGuid(),
                OriginId = Guid.NewGuid()
            };

            TaskCompletionSource<R> tcs = new TaskCompletionSource<R>();
            Timer timer = null;
            Guid guid;
            guid = await SubscribeAsync(async (BusEvent waitedEvent, R waitedMessage) =>
            {
                if (waitedEvent.OriginId == parent.OriginId)
                {
                    timer.Dispose();
                    await UnsubscribeAsync(guid);
                    tcs.SetResult(waitedMessage);
                }
            });
            timer = new Timer(state =>
            {
                timer.Dispose();
                UnsubscribeAsync(guid).Wait();
                tcs.TrySetException(new TimeoutException($"Request timed out."));
            }, null, timeout, TimeSpan.FromMilliseconds(-1));

            await PublishAsync(message, parent);

            return await tcs.Task;
        }
        
        public Task UnsubscribeAsync(Guid guid)
        {
            foreach(var topicHandler in topicHandlers)
            {
                topicHandler.Value.TryRemove(guid, out Func<BusEvent, Task> handler);
            }
            return Task.CompletedTask;
        }

        public List<Guid> GetSubscribers(BusTopic topic = null)
        {
            return topicHandlers
                .Where(th => topic == null || th.Key == topic)
                .SelectMany(th => th.Value)
                .Select(th => th.Key)
                .ToList();
        }


    }
}