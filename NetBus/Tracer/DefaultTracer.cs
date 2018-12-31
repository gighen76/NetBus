using NetBus.Bus;
using System;
using System.Threading.Tasks;

namespace NetBus.Tracer
{
    public class DefaultTracer : BaseTracer
    {

        private readonly BaseBus baseBus;

        public DefaultTracer(BaseBus baseBus)
        {
            this.baseBus = baseBus;
        }

        protected override async Task ExecuteTrace(TracerLog log)
        {
            if (log.LogType == TracerLogType.PUBLISH)
            {
                Console.WriteLine($"{log.Application.Name} -> {log.BusEvent.Topic.Name}: {log.BusEvent.Id}");
            }
            else if (log.LogType == TracerLogType.CONSUME)
            {
                Console.WriteLine($"{log.BusEvent.Topic.Name} -> {log.Application.Name}: {log.BusEvent.Id} ({log.ElapsedTime.Milliseconds}ms)");
            }
        }
    }
}