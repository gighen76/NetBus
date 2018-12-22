using System.Threading.Tasks;
using NetBus.Test.Application.Messages;
using NetBus.Subscriber;

namespace NetBus.Test.Application.Subscribers
{
    public class OrderCreatedSubscriber : ISubscriber<OrderCreated>
    {

        

        public OrderCreatedSubscriber()
        {
            
        }

        public async Task Execute(BusContext busContext, OrderCreated message)
        {

            if (message.Id.HasValue)
            {

            }
            


        }
    }
}