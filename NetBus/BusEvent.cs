using System;

namespace NetBus
{

    public class BusEvent
    {
        
        public Guid Id { get; set; }
        public Guid ParentId { get; set; }
        public Guid OriginId { get; set; }
        BusTopic Topic { get; set; }
        BusApplication Application { get; set; }

    }

}