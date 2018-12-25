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

        interface ITopicHandler
        {
            Task HandleAsync(BusApplication application, BusTopic topic, byte[] messageBytes);
            Task RemoveHandlerAsync(Guid guid);
            List<Guid> GetHandlers();
        }
        class TopicHandler<T> : ITopicHandler where T : class
        {

            private readonly ISerializer serializer;
            private readonly ITracer tracer;
            private readonly ConcurrentDictionary<Guid, Func<BusEvent<T>, Task>> handlers = new ConcurrentDictionary<Guid, Func<BusEvent<T>, Task>>();

            public TopicHandler(ISerializer serializer, ITracer tracer)
            {
                this.serializer = serializer;
                this.tracer = tracer;
            }

            public Task<Guid> AddHandlerAsync(Func<BusEvent<T>, Task> handler)
            {
                Guid handlerGuid = Guid.NewGuid();
                handlers.GetOrAdd(handlerGuid, handler);
                return Task.FromResult<Guid>(handlerGuid);
            }

            public Task RemoveHandlerAsync(Guid guid)
            {
                handlers.TryRemove(guid, out Func<BusEvent<T>, Task> handler);
                return Task.CompletedTask;
            }

            public List<Guid> GetHandlers()
            {
                return handlers.Keys.ToList();
            }


            public async Task HandleAsync(BusApplication application, BusTopic topic, byte[] messageBytes)
            {
                var busEvent = serializer.Deserialize<BusEvent<T>>(messageBytes);
                foreach(var handler in handlers)
                {
                    await handler.Value(busEvent);
                }
                await tracer.AddBusEventTraceAsync(application, topic, busEvent, TimeSpan.FromSeconds(1));

            }

        }


        private ConcurrentDictionary<BusTopic, ITopicHandler> topicHandlers = new ConcurrentDictionary<BusTopic, ITopicHandler>();

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

        public BusApplication Application => bus.Application;

        private async Task Bus_OnMessage(BusTopic topic, byte[] eventBytes)
        {
            if (topicHandlers.ContainsKey(topic))
            {
                await topicHandlers[topic].HandleAsync(Application, topic, eventBytes);
            }
        }

        public async Task PublishAsync<T>(BusEvent<T> busEvent) where T : class
        {
            var eventBytes = serializer.Serialize(busEvent);

            var topic = topicResolver.ResolveTopicName<T>();

            await bus.PublishAsync(topic, eventBytes);
            await tracer.RegisterBusEventAsync(Application, topic, busEvent);
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
        
        public async Task UnsubscribeAsync<T>(Guid guid) where T: class
        {
            var topic = topicResolver.ResolveTopicName<T>();

            if (topicHandlers.ContainsKey(topic))
            {
                await topicHandlers[topic].RemoveHandlerAsync(guid);
            }
        }

        public async Task<Guid> SubscribeAsync<T>(Func<BusEvent<T>, Task> handler) where T : class
        {
            var topic = topicResolver.ResolveTopicName<T>();

            var topicHandler = (TopicHandler<T>)this.topicHandlers.GetOrAdd(topic, new TopicHandler<T>(serializer, tracer));
            var handlerGuid = await topicHandler.AddHandlerAsync(handler);

            await bus.SubscribeAsync(topic);
            return handlerGuid;
        }



        public List<Guid> GetSubscribers<T>() where T: class
        {
            List<Guid> result = new List<Guid>();

            var topic = topicResolver.ResolveTopicName<T>();
            if (topicHandlers.ContainsKey(topic))
            {
                result.AddRange(topicHandlers[topic].GetHandlers());
            }
            return result;
        }


    }
}