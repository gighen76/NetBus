using System;

namespace NetBus.Test.Messages
{
    public class DeserializeMessageTest
    {

        public int? Id { get; set; }
        public int OptionalId { get; set; }

        public decimal OptionalDecimal { get; set; }

        public Guid NotSerializedGuid { get; set; }

        public string NotSerializedString { get; set; }

    }
}