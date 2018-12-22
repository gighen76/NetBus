using NetBus.Bus;
using System;
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

        public override async Task PublishAsync(BusTopic topic, byte[] message)
        {
            if (SubscribedTopics.Contains(topic))
            {
                await ProcessMessage(topic, message);
            }
        }

        public override Task SubscribeAsync(BusTopic topic)
        {
            SubscribedTopics.Add(topic);
            return Task<bool>.FromResult(true);
        }

    }
}