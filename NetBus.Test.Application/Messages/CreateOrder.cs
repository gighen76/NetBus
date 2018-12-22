using NetBus;
namespace NetBus.Test.Application.Messages
{
    [BusTopic("CreateOrder")]
    public class CreateOrder
    {

        public int? Id { get; set; }

    }
}