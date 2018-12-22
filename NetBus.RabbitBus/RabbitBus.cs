using NetBus.Bus;
using NetBus.RabbitBus.Topology;
using NetBus.RabbitBus.Publisher;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NetBus.RabbitBus.Consumer;

namespace NetBus.RabbitBus
{
    public class RabbitBus : BaseBus
    {

        private readonly RabbitBusConfiguration configuration;
        private readonly IPublisher publisher;
        private readonly IConsumer consumer;

        public RabbitBus(RabbitBusConfiguration configuration, IPublisher publisher, IConsumer consumer) : base(configuration)
        {
            this.configuration = configuration;
            this.publisher = publisher;
            this.consumer = consumer;
            this.consumer.OnMessage += Consumer_OnMessage;

        }

        private async Task Consumer_OnMessage(IDictionary<string, string> headers, byte[] message)
        {
            if (headers.ContainsKey("TopicName"))
            {
                await this.ProcessMessage(headers["TopicName"], message);
            }
        }

        public override Task PublishAsync(string topicName, byte[] message)
        {
            var headers = new Dictionary<string, string>
            {
                { "TopicName", topicName }
            };

            return publisher.PublishAsync(SubscriberName, topicName, headers, message);

        }

        public override Task SubscribeAsync(string topicName)
        {
            return consumer.SubscribeAsync(SubscriberName, topicName, configuration.PrefetchCount);
        }
    }
}
