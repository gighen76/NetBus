using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace NetBus.Tracer
{
    public abstract class BaseTracer
    {

        private readonly BlockingCollection<TracerLog> logs =
            new BlockingCollection<TracerLog>(new ConcurrentQueue<TracerLog>());

        public BaseTracer()
        {
            Task.Run(async () => await ProcessLogs());
        }

        public void TracePublish(BusApplication application, BusEvent busEvent)
        {
            logs.Add(new TracerLog
            {
                LogType = TracerLogType.PUBLISH,
                Application = application,
                BusEvent = busEvent
            });
        }
        public void TraceConsume(BusApplication application, BusEvent busEvent, TimeSpan elapsedTime)
        {
            logs.Add(new TracerLog
            {
                LogType = TracerLogType.CONSUME,
                Application = application,
                BusEvent = busEvent,
                ElapsedTime = elapsedTime
            });
        }

        private async Task ProcessLogs()
        {
            while(true)
            {
                var tracerLog = logs.Take();
                await ExecuteTrace(tracerLog);
            }
        }

        protected abstract Task ExecuteTrace(TracerLog log);

    }
}