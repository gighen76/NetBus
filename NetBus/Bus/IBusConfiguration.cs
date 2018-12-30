using System;

namespace NetBus.Bus
{
    public interface IBusConfiguration : IServicesConfigurator
    {

        BusApplication Application { get; set; }

        TimeSpan WaitTimeout { get; set; }

        BusTopic TracerTopic { get; set; }

    }
}