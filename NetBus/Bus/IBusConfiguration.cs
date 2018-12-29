namespace NetBus.Bus
{
    public interface IBusConfiguration : IServicesConfigurator
    {

        BusApplication Application { get; set; }

        BusTopic TracerTopic { get; set; }

    }
}