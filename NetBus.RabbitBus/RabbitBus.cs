using NetBus.Bus;
using NetBus.RabbitBus.Consumer;
using NetBus.RabbitBus.Publisher;
using System.Collections.Generic;
using System.Threading.Tasks;

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
            await this.ProcessMessage(message, headers);
        }

        protected override Task ConcretePublishAsync(BusTopic topic, byte[] message, IDictionary<string, string> headers)
        {

            return publisher.PublishAsync(configuration.Application.Name, topic.Name, headers, message);

        }

        protected override Task ConcreteSubscribeAsync(BusTopic topic)
        {
            return consumer.SubscribeAsync(configuration.Application.Name, topic.Name, configuration.PrefetchCount);
        }
    }
}
