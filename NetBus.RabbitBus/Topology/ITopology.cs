namespace NetBus.RabbitBus.Topology
{
    public interface ITopology
    {

        string GetExchangeType();

        string GetExchangeName(string subscriberName, string topicName);

        string GetRoutingKey(string subscriberName, string topicName);

        string GetQueueName(string subcriberName, string topicName);

    }
}