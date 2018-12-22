using System.Threading.Tasks;

namespace NetBus.Subscriber
{

    public interface ISubscriber<T> where T : class
    {

         Task Execute(BusContext busContext, T message);

    }

}