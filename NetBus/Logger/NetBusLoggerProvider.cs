﻿using Microsoft.Extensions.Logging;
using NetBus.TopicResolver;
using System;
using System.Collections.Concurrent;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace NetBus.Logger
{
    public class NetBusLoggerProvider : ILoggerProvider
    {

        private readonly BlockingCollection<NetBusLog> logs = 
            new BlockingCollection<NetBusLog>(new ConcurrentQueue<NetBusLog>());

        private class NetBusLogger : ILogger
        {
            private readonly NetBusLoggerProvider provider;
            private readonly string categoryName;

            public NetBusLogger(NetBusLoggerProvider provider, string categoryName)
            {
                this.provider = provider;
                this.categoryName = categoryName;
            }

            public IDisposable BeginScope<TState>(TState state)
            {
                throw new NotImplementedException();
            }

            public bool IsEnabled(LogLevel logLevel) => true;

            public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
            {
                NetBusLog log = state as NetBusLog;
                if (log != null)
                {
                    provider.logs.Add(log);
                }
            }
        }


        

        public NetBusLoggerProvider()
        {
            StartProcessLog();
        }

        private void StartProcessLog()
        {


            Task.Run(async () =>
            {

                while (true)
                {
                    var log = logs.Take();

                    try
                    {

                    }
                    catch
                    {
                        logs.Add(log);
                    }
                }
            });
        }

        public ILogger CreateLogger(string categoryName)
        {
            return new NetBusLogger(this, categoryName);
        }

        public void Dispose()
        {
            logs.Dispose();
        }
    }
}
