using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NetBus.Host;
using NetBus.RabbitBus;
using NetBus.Logger;
using System;

namespace NetBus.Test.Application
{
    class Program
    {

        public static void Main(string[] args)
        {

            new HostBuilder()
                .UseNetBusService<RabbitBusConfiguration,Configuration>(configuration =>
                {
                    configuration.Application = new BusApplication("test");
                    configuration.PrefetchCount = 10;
                    configuration.Uri = "amqp://njelcwjj:cnqSAr1DUt1C5JQd6o3ybAj0uCrP1f0S@flamingo.rmq.cloudamqp.com/njelcwjj";
                    configuration.RecoveryInterval = TimeSpan.FromMinutes(5);
                })
                .ConfigureLogging((hostContext, configLogging) =>
                {
                    configLogging.SetMinimumLevel(LogLevel.Trace);
                    configLogging.AddConsole();
                    configLogging.AddDebug();
                    configLogging.AddNetBusLogger();
                })
                .Build()
                .Run();

        }
    }
}
