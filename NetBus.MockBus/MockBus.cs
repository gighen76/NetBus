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

        private HashSet<string> subscribedTopics = new HashSet<string>();

        public override async Task PublishAsync(string topicName, byte[] message)
        {
            if (subscribedTopics.Contains(topicName))
            {
                await ProcessMessage(topicName, message);
            }
        }

        public override Task SubscribeAsync(string topicName)
        {
            subscribedTopics.Add(topicName);
            return Task<bool>.FromResult(true);
        }

    }
}