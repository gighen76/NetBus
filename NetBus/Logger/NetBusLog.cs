using System;
using System.Collections.Generic;

namespace NetBus.Logger
{

    public class NetBusLog
    {

        public NetBusLogType LogType { get; set; }
        public string ApplicationName { get; set; }

        public byte[] Message { get; set; }
        public IDictionary<string, string> BusHeaders { get; set; }

        public TimeSpan ElapsedTime { get; set; }
        public Exception Exception { get; set; }

        public BusEvent GetBusEvent()
        {
            return new BusEvent(Message, BusHeaders);
        }


    }
}