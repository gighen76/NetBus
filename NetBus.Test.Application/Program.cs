﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using NetBus.Host;
using NetBus.MockBus;
using System.IO;
using NetBus.RabbitBus;
using System;

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
                    configuration.SubscriberName = "test";
                    configuration.PrefetchCount = 10;
                    configuration.Uri = "amqp://njelcwjj:cnqSAr1DUt1C5JQd6o3ybAj0uCrP1f0S@flamingo.rmq.cloudamqp.com/njelcwjj";
                    configuration.RecoveryInterval = TimeSpan.FromMinutes(5);
                })
                .Build()
                
                .Run();

        }
    }
}