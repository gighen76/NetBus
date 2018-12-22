namespace NetBus.Bus
{
    public interface IBusConfiguration : IServicesConfigurator
    {

        string SubscriberName { get; set; }

    }
}