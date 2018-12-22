using System;

namespace NetBus
{

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class BusTopicAttribute : Attribute
    {
        public BusTopicAttribute(string name)
        {
            Name = name;
        }

        public string Name { get; }

    }
}