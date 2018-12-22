using RabbitMQ.Client;

namespace NetBus.RabbitBus.Topology
{
    public class DefaultTopology : ITopology
    {


        public string GetExchangeType()
        {
            return ExchangeType.Direct;
        }

        public string GetExchangeName(string subscriberName, string topicName)
        {
            return topicName;
        }

        public string GetRoutingKey(string subscriberName, string topicName)
        {
            return topicName;
        }

        public string GetQueueName(string subcriberName, string topicName)
        {
            return $"{subcriberName}:{topicName}";
        }

    }
}