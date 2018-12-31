namespace NetBus.TopicResolver
{
    public interface ITopicResolver
    {

        BusTopic ResolveTopicName<T>();

    }
}