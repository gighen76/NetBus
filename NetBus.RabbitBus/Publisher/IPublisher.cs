using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NetBus.RabbitBus.Publisher
{
    public interface IPublisher : IDisposable
    {

        Task PublishAsync(string subscriberName, string topicName, IDictionary<string, string> headers, byte[] message);

    }
}