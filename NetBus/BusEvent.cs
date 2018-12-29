using System;

namespace NetBus
{

    public class BusEvent
    {
        
        public Guid Id { get; set; }
        public Guid ParentId { get; set; }
        public Guid OriginId { get; set; }
        public BusTopic Topic { get; set; }
        public BusApplication Application { get; set; }

        public byte[] Message { get; set; }

    }

}