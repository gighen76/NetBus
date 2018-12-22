using System;
using System.Threading.Tasks;

namespace NetBus.Tracer
{
    public interface ITracer
    {

        Task RegisterBusEventAsync<T>(string senderSubscriberName, string topicName, BusEvent<T> busEvent) where T: class;
        Task AddBusEventTraceAsync(string receiverSubscriberName, BusEvent busEvent, TimeSpan timeSpan);


    }
}