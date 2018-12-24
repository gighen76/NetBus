using System;
using System.Threading.Tasks;

namespace NetBus.Tracer
{
    public interface ITracer
    {

        Task RegisterBusEventAsync<T>(BusApplication senderApplication, BusTopic topic, BusEvent<T> busEvent) where T: class;
        Task AddBusEventTraceAsync(BusApplication receiverApplication, BusTopic topic, BusEvent busEvent, TimeSpan timeSpan);


    }
}