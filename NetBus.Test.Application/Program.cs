using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using NetBus.Host;
using NetBus.MockBus;
using System.IO;
using NetBus.RabbitBus;
using System;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace NetBus.Test.Application
{
    class Program
    {

        public static void Main(string[] args)
        {

            new HostBuilder()
                .ConfigureHostConfiguration(configHost =>
                {
                    configHost.SetBasePath(Directory.GetCurrentDirectory());
                    configHost.AddJsonFile("hostsettings.json", optional: true);
                    configHost.AddEnvironmentVariables(prefix: "PREFIX_");
                    configHost.AddCommandLine(args);
                })
                .ConfigureAppConfiguration((hostContext, configApp) =>
                {
                    configApp.AddJsonFile("appsettings.json", optional: true);
                    configApp.AddJsonFile(
                        $"appsettings.{hostContext.HostingEnvironment.EnvironmentName}.json",
                        optional: true);
                    configApp.AddEnvironmentVariables(prefix: "PREFIX_");
                    configApp.AddCommandLine(args);
                })
                .UseNetBusService<RabbitBusConfiguration,Configuration>(configuration =>
                {
                    configuration.Application = new BusApplication("test");
                    //configuration.TracerTopic = new BusTopic("tracer");
                    configuration.PrefetchCount = 10;
                    configuration.Uri = "amqp://njelcwjj:cnqSAr1DUt1C5JQd6o3ybAj0uCrP1f0S@flamingo.rmq.cloudamqp.com/njelcwjj";
                    configuration.RecoveryInterval = TimeSpan.FromMinutes(5);
                })
                
                .ConfigureLogging((hostContext, configLogging) =>
                {
                    configLogging.SetMinimumLevel(LogLevel.Trace);
                    configLogging.AddConsole();
                    configLogging.AddDebug();
                })
                
                .Build()
                
                .Run();

        }
    }
}
