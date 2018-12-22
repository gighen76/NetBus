using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NetBus.RabbitBus.Consumer
{
    public interface IConsumer : IDisposable
    {

        event Func<IDictionary<string, string>, byte[], Task> OnMessage;

        Task SubscribeAsync(string subscriberName, string topicName, ushort prefetchCount);


    }
}