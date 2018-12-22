using System.Threading.Tasks;
using NetBus.Test.Application.Messages;
using NetBus.Subscriber;

namespace NetBus.Test.Application.Subscribers
{
    public class CreateOrderSubscriber : ISubscriber<CreateOrder>
    {

        

        public CreateOrderSubscriber()
        {
            
        }

        public async Task Execute(BusContext busContext, CreateOrder message)
        {
;
            await busContext.PublishAsync(new OrderCreated
            {
                Id = message.Id
            });


        }
    }
}