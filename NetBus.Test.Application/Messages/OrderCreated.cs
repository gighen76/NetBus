namespace NetBus.Test.Application.Messages
{

    [BusTopic("OrderCreated")]
    public class OrderCreated
    {

        public int? Id { get; set; }

    }
}