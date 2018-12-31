using NetBus.Bus;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NetBus.MockBus
{
    public class MockBus : BaseBus
    {

        public MockBus(MockBusConfiguration configuration) : base(configuration)
        {

        }

        public HashSet<BusTopic> SubscribedTopics = new HashSet<BusTopic>();

        protected override async Task ConcretePublishAsync(BusTopic topic, byte[] message, IDictionary<string, string> headers)
        {
            if (SubscribedTopics.Contains(topic))
            {
                await ProcessMessage(message, headers);
            }
        }

        protected override Task ConcreteSubscribeAsync(BusTopic topic)
        {
            SubscribedTopics.Add(topic);
            return Task<bool>.FromResult(true);
        }

    }
}