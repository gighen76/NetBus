using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;

namespace NetBus.Logger
{
    public static class NetBusLoggerExtensions
    {


        public static ILoggingBuilder AddNetBusLogger(this ILoggingBuilder builder)
        {
            builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<ILoggerProvider, NetBusLoggerProvider>());
            return builder;
        }

    }
}