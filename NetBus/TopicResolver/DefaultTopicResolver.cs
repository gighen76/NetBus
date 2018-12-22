using System;
using System.Linq;

namespace NetBus.TopicResolver
{
    public class DefaultTopicResolver : ITopicResolver
    {
        public string ResolveTopicName<T>()
        {
            Type type = typeof(T);

            var topicAttribute = type.CustomAttributes.Where(ca => ca.AttributeType == typeof(BusTopicAttribute)).SingleOrDefault();
            if (topicAttribute != null)
            {
                var name = topicAttribute.ConstructorArguments[0].Value.ToString();
                if (name != null)
                {
                    return name;
                }
            }

            return type.Name;

        }
    }
}