using System.Threading.Tasks;
using NetBus.Subscriber;
using NetBus.Test.Messages;

namespace NetBus.Test.Subscriber
{
    class TestMessageSubscriber : ISubscriber<TestMessage>
    {
        public Task Execute(BusContext busContext, TestMessage message)
        {
            throw new System.NotImplementedException();
        }
    }
}