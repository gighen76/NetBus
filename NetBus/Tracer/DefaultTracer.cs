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

        public Task RegisterBusEventAsync<T>(string senderSubscriberName, BusTopic topic, BusEvent<T> busEvent) where T : class
        {
            Console.WriteLine($"{senderSubscriberName} -> {topic.Name} : #{busEvent.Id}# {busEvent.ParentId} {busEvent.OriginId}");
            return Task.CompletedTask;
        }

        public Task AddBusEventTraceAsync(string receiverSubscriberName, BusEvent busEvent, TimeSpan timeSpan)
        {
            Console.WriteLine($"-> {receiverSubscriberName} : #{busEvent.Id}# {busEvent.ParentId} {busEvent.OriginId}");
            return Task.CompletedTask;
        }


    }
}