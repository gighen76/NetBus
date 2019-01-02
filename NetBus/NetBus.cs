using Microsoft.Extensions.Logging;
using NetBus.Bus;
using NetBus.Serializer;
using NetBus.TopicResolver;
using NetBus.Logger;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace NetBus
{
    public class NetBus
    {

        private ConcurrentDictionary<BusTopic, ConcurrentDictionary<Guid,Func<BusEvent,Task>>> topicHandlers = 
            new ConcurrentDictionary<BusTopic, ConcurrentDictionary<Guid, Func<BusEvent, Task>>>();

        private readonly BaseBus bus;
        private readonly ISerializer serializer;
        private readonly ITopicResolver topicResolver;
        private readonly ILogger logger;

        public NetBus(BaseBus bus, ISerializer serializer, 
            ITopicResolver topicResolver, ILogger<NetBus> logger)
        {
            
            this.bus = bus;
            this.bus.OnMessage += Bus_OnMessage;
            this.serializer = serializer;
            this.topicResolver = topicResolver;
            this.logger = logger;
        }

        private async Task Bus_OnMessage(BusEvent busEvent)
        {
            Stopwatch stopWatch = Stopwatch.StartNew();
            Exception handlerException = null;
            if (topicHandlers.ContainsKey(busEvent.Topic))
            {
                try
                {
                    foreach (var handler in topicHandlers[busEvent.Topic])
                    {
                        await handler.Value(busEvent);
                    }
                }
                catch(Exception ex)
                {
                    handlerException = ex;
                }

            }

            logger.Log(LogLevel.Trace, new EventId(2), new NetBusLog
            {
                ApplicationName = bus.Configuration.Application.Name,
                Message = busEvent.Message,
                BusHeaders = busEvent.GetBusHeaders(),
                ElapsedTime = stopWatch.Elapsed,
                LogType = NetBusLogType.CONSUME,
                Exception = handlerException
            }, null, (tl, e) => { var be = tl.GetBusEvent(); return $"{be.Topic} -> {tl.ApplicationName}"; });

        }

        public async Task PublishAsync<T>(T message, BusEvent parentEvent = null) where T : class
        {
            var messageBytes = serializer.Serialize(message);
            var topic = topicResolver.ResolveTopicName<T>();
            var busEvent = new BusEvent(messageBytes, topic, parentEvent);
            await bus.PublishAsync(busEvent);
 
            logger.Log(LogLevel.Trace, new EventId(1), new NetBusLog
            {
                ApplicationName = bus.Configuration.Application.Name,
                Message = busEvent.Message,
                BusHeaders = busEvent.GetBusHeaders(),
                LogType = NetBusLogType.PUBLISH
            }, null, (tl, e) => { var be = tl.GetBusEvent(); return $"{be.Topic} -> {tl.ApplicationName}"; });
            
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

        public async Task<R> PublishAndWaitAsync<T, R>(T message, BusEvent parentEvent = null) where T : class where R : class
        {

            var messageBytes = serializer.Serialize(message);

            var topic = topicResolver.ResolveTopicName<T>();
            var waitForTopic = topicResolver.ResolveTopicName<R>();

            var waitedEvent = await bus.PublishAndWaitAsync(new BusEvent(messageBytes, topic, parentEvent), waitForTopic);

            return serializer.Deserialize<R>(waitedEvent.Message);

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