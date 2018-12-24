using NetBus.Bus;
using System;
using System.Threading.Tasks;

namespace NetBus.Tracer
{
    public class DefaultTracer : ITracer
    {

        public DefaultTracer()
        {
        }

        public Task RegisterBusEventAsync<T>(BusApplication senderApplication, BusTopic topic, BusEvent<T> busEvent) where T : class
        {
            Console.WriteLine($"{senderApplication.Name} -> {topic.Name} : #{busEvent.Id}# {busEvent.ParentId} {busEvent.OriginId}");
            return Task.CompletedTask;
        }

        public Task AddBusEventTraceAsync(BusApplication receiverApplication, BusTopic topic, BusEvent busEvent, TimeSpan timeSpan)
        {
            Console.WriteLine($"{topic.Name} -> {receiverApplication.Name} : #{busEvent.Id}# {busEvent.ParentId} {busEvent.OriginId}");
            return Task.CompletedTask;
        }


    }
}