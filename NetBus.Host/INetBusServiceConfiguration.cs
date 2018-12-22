using System.Threading.Tasks;

namespace NetBus.Host
{
    public interface INetBusServiceConfiguration
    {

        Task OnStarted();

    }
}