namespace NetBus.Bus
{
    public interface IBusConfiguration : IServicesConfigurator
    {

        BusApplication Application { get; set; }

    }
}