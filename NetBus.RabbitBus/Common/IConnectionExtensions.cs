using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Threading.Tasks;

namespace NetBus.RabbitBus.Common
{
    public static class IConnectionExtensions
    {

        static public Task<bool> EnsureConnectionAsync(this IConnection connection)
        {

            if (connection.IsOpen)
            {
                return Task.FromResult(true);
            }

            if (connection.CloseReason != null && connection.CloseReason.Initiator != ShutdownInitiator.Application)
            {
                var tcs = new TaskCompletionSource<bool>();

                EventHandler<EventArgs> recoverySucceeded = null;
                recoverySucceeded = (sender, args) =>
                {
                    tcs.TrySetResult(true);
                    connection.RecoverySucceeded -= recoverySucceeded;
                };
                connection.RecoverySucceeded += recoverySucceeded;

                EventHandler<ConnectionRecoveryErrorEventArgs> connectionRecoveryError = null;
                connectionRecoveryError = (sender, args) =>
                {
                    tcs.TrySetException(args.Exception);
                    connection.ConnectionRecoveryError -= connectionRecoveryError;
                };
                connection.ConnectionRecoveryError += connectionRecoveryError;

                return tcs.Task;
            }

            throw new InvalidOperationException("Unable to recover connection");

        }


    }
}