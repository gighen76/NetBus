using System;

namespace NetBus.TopicResolver
{
    public interface ITopicResolver
    {

        string ResolveTopicName<T>();

    }
}