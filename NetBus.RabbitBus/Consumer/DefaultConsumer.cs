using RabbitMQ.Client;
using System.Threading;
using System.Threading.Tasks;
using NetBus.RabbitBus.Common;
using RabbitMQ.Client.Events;
using System;
using NetBus.RabbitBus.Topology;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Collections.Generic;

namespace NetBus.RabbitBus.Consumer
{
    public class DefaultConsumer : IConsumer
    {
        private SemaphoreSlim @lock = new SemaphoreSlim(1, 1);

        private readonly IConnectionFactory connectionFactory;
        private readonly ITopology topology;
        private IConnection connection;
        private IModel channel;

        private ConcurrentDictionary<string, AsyncEventingBasicConsumer> consumers = new ConcurrentDictionary<string, AsyncEventingBasicConsumer>();

        public readonly object m_eventLock = new object();
        private Func<IDictionary<string,string>, byte[], Task> _OnMessage;
        public event Func<IDictionary<string, string>, byte[], Task> OnMessage
        {
            add
            {
                lock (m_eventLock)
                {
                    _OnMessage += value;
                }
            }
            remove
            {
                lock (m_eventLock)
                {
                    _OnMessage -= value;
                }
            }
        }


        public DefaultConsumer(IConnectionFactory connectionFactory, ITopology topology)
        {
            this.connectionFactory = connectionFactory;
            this.topology = topology;
            connection = null;
            channel = null;
        }

        public async Task SubscribeAsync(string subscriberName, string topicName, ushort prefetchCount)
        {
            await @lock.WaitAsync().ConfigureAwait(false);
            try
            {

                var exchangeName = topology.GetExchangeName(subscriberName, topicName);
                var queueName = topology.GetQueueName(subscriberName, topicName);
                var routingKey = topology.GetRoutingKey(subscriberName, topicName);
                var exchangeType = topology.GetExchangeType();

                connection = connection ?? connectionFactory.CreateConnection();
                await this.connection.EnsureConnectionAsync();

                channel = channel ?? this.connection.CreateModel();
                channel.ExchangeDeclare(exchangeName, exchangeType, true, false);
                channel.QueueDeclare(queueName, true, false, false);
                channel.QueueBind(queueName, exchangeName, routingKey);
                

                consumers.GetOrAdd(queueName, qn =>
                {
                    var consumerChannel = connection.CreateModel();
                    var consumer = new AsyncEventingBasicConsumer(consumerChannel);
                    consumer.Received += async (ch, ea) =>
                    {
                        consumerChannel.BasicAck(ea.DeliveryTag, false);
                        if (_OnMessage != null && ea.BasicProperties.Headers != null)
                        {                             
                            var headers = ea.BasicProperties.Headers.ToDictionary(h => h.Key, h => Encoding.UTF8.GetString((byte[])h.Value));
                            await _OnMessage(headers, ea.Body);
                        }
                    };
                    consumerChannel.BasicQos(0, prefetchCount, false);
                    consumerChannel.BasicConsume(queueName, false, consumer);
                    return consumer;
                });

            }
            finally
            {
                @lock.Release();
            }
        }

        public void Dispose()
        {
            if (channel != null)
            {
                channel.Dispose();
            }
            if (connection != null)
            {
                connection.Dispose();
            }
        }
    }
}