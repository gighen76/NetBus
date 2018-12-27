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

        private ConcurrentDictionary<BusTopic, ConcurrentDictionary<Guid,Func<byte[],Task>>> topicHandlers = 
            new ConcurrentDictionary<BusTopic, ConcurrentDictionary<Guid, Func<byte[], Task>>>();

        private readonly BaseBus bus;
        private readonly ISerializer serializer;
        private readonly ITopicResolver topicResolver;
        private readonly ILogger logger;

        public NetBus(BaseBus bus, ISerializer serializer, 
            ITopicResolver topicResolver, ILogger<NetBus> logger = null)
        {
            
            this.bus = bus;
            this.bus.OnMessage += Bus_OnMessage;
            this.serializer = serializer;
            this.topicResolver = topicResolver;
            this.logger = logger;
            
        }

        public BusApplication Application => bus.Application;

        private async Task Bus_OnMessage(BusTopic topic, byte[] eventBytes)
        {
            if (topicHandlers.ContainsKey(topic))
            {
                foreach (var handler in topicHandlers[topic])
                {
                    await handler.Value(eventBytes);
                }
            }
        }

        public async Task PublishAsync<T>(BusEvent<T> busEvent) where T : class
        {
            var eventBytes = serializer.Serialize(busEvent);

            var topic = topicResolver.ResolveTopicName<T>();

            await bus.PublishAsync(topic, eventBytes);
            logger.LogPublish(Application, topic, busEvent);
        }

        public Task PublishAsync<T>(T message, BusEvent parent = null) where T : class
        {
            var busEvent = new BusEvent<T>(message, parent);

            return PublishAsync(busEvent);
        }
        
        public async Task<R> PublishAndWaitAsync<T, R>(T message, TimeSpan timeout, BusEvent parent = null) where T : class where R : class
        {
            var busEvent = new BusEvent<T>(message, parent);

            TaskCompletionSource<R> tcs = new TaskCompletionSource<R>();
            Timer timer = null;
            Guid guid;
            guid = await SubscribeAsync(async (BusEvent<R> waitedEvent) =>
            {
                if (waitedEvent.OriginId == busEvent.OriginId)
                {
                    timer.Dispose();
                    await UnsubscribeAsync(guid);
                    tcs.SetResult(waitedEvent.Message);
                }
            });
            timer = new Timer(state =>
            {
                timer.Dispose();
                UnsubscribeAsync(guid).Wait();
                tcs.TrySetException(new TimeoutException($"Request timed out."));
            }, null, timeout, TimeSpan.FromMilliseconds(-1));

            await PublishAsync(busEvent);

            return await tcs.Task;
        }
        
        public Task UnsubscribeAsync(Guid guid)
        {
            foreach(var topicHandler in topicHandlers)
            {
                topicHandler.Value.TryRemove(guid, out Func<byte[], Task> handler);
            }
            return Task.CompletedTask;
        }

        public async Task<Guid> SubscribeAsync<T>(Func<BusEvent<T>, Task> handler) where T : class
        {
            var topic = topicResolver.ResolveTopicName<T>();

            var topicHandler = this.topicHandlers.GetOrAdd(topic, 
                new ConcurrentDictionary<Guid, Func<byte[], Task>>());

            Guid handlerGuid = Guid.NewGuid();
            topicHandler.GetOrAdd(handlerGuid, async (messageBytes) =>
            {
                var busEvent = serializer.Deserialize<BusEvent<T>>(messageBytes);
                await handler(busEvent);
                logger.LogConsume(Application, topic, busEvent);
            });

            await bus.SubscribeAsync(topic);
            return handlerGuid;
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