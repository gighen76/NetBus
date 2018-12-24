using NetBus.Serializer;
using NetBus.TopicResolver;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using System.Linq;
using System.Threading;
using System.Collections.Generic;
using NetBus.Tracer;
using Microsoft.Extensions.Options;
using NetBus.Bus;
using NetBus.Subscriber;
using Microsoft.Extensions.DependencyInjection;

namespace NetBus
{
    public class NetBus
    {

        private ConcurrentDictionary<BusTopic, ConcurrentDictionary<Guid, Func<byte[], Task>>> handlers = new ConcurrentDictionary<BusTopic, ConcurrentDictionary<Guid, Func<byte[], Task>>>();

        private readonly BaseBus bus;
        private readonly ISerializer serializer;
        private readonly ITopicResolver topicResolver;
        private readonly ITracer tracer;

        public NetBus(BaseBus bus, ISerializer serializer, 
            ITopicResolver topicResolver, ITracer tracer)
        {
            
            this.bus = bus;
            this.bus.OnMessage += Bus_OnMessage;
            this.serializer = serializer;
            this.topicResolver = topicResolver;
            this.tracer = tracer;
            
        }

        public string SubscriberName => bus.SubscriberName;

        private async Task Bus_OnMessage(BusTopic topic, byte[] eventBytes)
        {
            if (handlers.ContainsKey(topic))
            {
                foreach(var handler in handlers[topic].Values)
                {
                    await handler(eventBytes);
                }
            }
        }

        public async Task PublishAsync<T>(BusEvent<T> busEvent) where T : class
        {
            var eventBytes = serializer.Serialize(busEvent);

            var topic = topicResolver.ResolveTopicName<T>();

            await bus.PublishAsync(topic, eventBytes);
            await tracer.RegisterBusEventAsync(SubscriberName, topic, busEvent);
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
                    await UnsubscribeAsync<R>(guid);
                    tcs.SetResult(waitedEvent.Message);
                }
            });
            timer = new Timer(state =>
            {
                timer.Dispose();
                UnsubscribeAsync<R>(guid).Wait();
                tcs.TrySetException(new TimeoutException($"Request timed out."));
            }, null, timeout, TimeSpan.FromMilliseconds(-1));

            await PublishAsync(busEvent);

            return await tcs.Task;
        }
        
        public Task UnsubscribeAsync<T>(Guid guid)
        {
            var topic = topicResolver.ResolveTopicName<T>();
            var topicHandler = handlers.Where(h => h.Key == topic).Select(h => h.Value).SingleOrDefault();
            if (topicHandler != null)
            {
                topicHandler.TryRemove(guid, out Func<byte[], Task> handler);
            }
            
            return Task.FromResult(true);
        }

        public async Task<Guid> SubscribeAsync<T>(Func<BusEvent<T>, Task> handler) where T : class
        {
            var topic = topicResolver.ResolveTopicName<T>();

            var topicHandlers = handlers.GetOrAdd(topic, new ConcurrentDictionary<Guid, Func<byte[], Task>>());
            var handlerGuid = Guid.NewGuid();

            topicHandlers.GetOrAdd(handlerGuid, async (byte[] messageBytes) =>
            {
                var busEvent = serializer.Deserialize<BusEvent<T>>(messageBytes);
                try
                {
                    
                    await handler(busEvent);
                    await tracer.AddBusEventTraceAsync(SubscriberName, busEvent, TimeSpan.FromMilliseconds(1));
                }
                catch (Exception ex)
                {

                }
                
            });

            await bus.SubscribeAsync(topic);
            return handlerGuid;
        }



        public List<Guid> GetSubscribers<T>()
        {
            List<Guid> result = new List<Guid>();

            var topic = topicResolver.ResolveTopicName<T>();
            if (handlers.ContainsKey(topic))
            {
                result.AddRange(handlers[topic].Keys);
            }
            return result;
        }


    }
}