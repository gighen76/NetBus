using System;

namespace NetBus
{

    public abstract class BusEvent
    {
        public Guid Id { get; set; }
        public Guid? ParentId { get; set; }
        public Guid OriginId { get; set; }

        public BusEvent(BusEvent parent = null)
        {
            Id = Guid.NewGuid();
            ParentId = parent?.Id;
            OriginId = parent?.OriginId ?? Id;
        }
    }

    public class BusEvent<T> : BusEvent where T : class 
    {

        public BusEvent(T message, BusEvent parent = null) : base(parent)
        {
            Message = message;
        }

        public T Message { get; set; }

    }
}