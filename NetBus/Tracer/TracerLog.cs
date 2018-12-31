using System;

namespace NetBus.Tracer
{
    public class TracerLog
    {

        public TracerLogType LogType { get; set; }
        public BusApplication Application { get; set; }
        public BusEvent BusEvent { get; set; }
        public TimeSpan ElapsedTime { get; set; }

    }
}