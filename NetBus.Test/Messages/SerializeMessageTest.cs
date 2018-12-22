using System;

namespace NetBus.Test.Messages
{
    public class SerializeMessageTest
    {

        public int Id { get; set; }
        public int? OptionalId { get; set; }
        public Guid Guid { get; set; }
        public Guid? OptionalGuid { get; set; }
        public string String { get; set; }
        public decimal Decimal { get; set; }
        public decimal? OptionalDecimal { get; set; }

        public override bool Equals(object obj)
        {
            if (obj is SerializeMessageTest)
            {
                var msg = obj as SerializeMessageTest;

                return msg.Id == Id && msg.OptionalId == OptionalId &&
                       msg.Guid == Guid && msg.OptionalGuid == OptionalGuid &&
                       msg.String == String && msg.Decimal == Decimal &&
                       msg.OptionalDecimal == OptionalDecimal;
            }
            return false;
        }

    }
}