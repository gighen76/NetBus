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
                await this.ProcessMessage(new BusTopic(headers["TopicName"]), message);
            }
        }

        public override Task PublishAsync(BusTopic topic, byte[] message)
        {
            var headers = new Dictionary<string, string>
            {
                { "TopicName", topic.Name }
            };

            return publisher.PublishAsync(Application.Name, topic.Name, headers, message);

        }

        public override Task SubscribeAsync(BusTopic topic)
        {
            return consumer.SubscribeAsync(Application.Name, topic.Name, configuration.PrefetchCount);
        }
    }
}
