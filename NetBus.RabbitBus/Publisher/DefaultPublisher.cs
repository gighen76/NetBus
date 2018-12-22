using System.Collections.Generic;
using System.Threading.Tasks;
using RabbitMQ.Client;
using NetBus.RabbitBus.Common;
using System.Threading;
using System.Linq;
using NetBus.RabbitBus.Topology;

namespace NetBus.RabbitBus.Publisher
{
    public class DefaultPublisher : IPublisher
    {

        private SemaphoreSlim @lock = new SemaphoreSlim(1, 1);

        private readonly IConnectionFactory connectionFactory;
        private readonly ITopology topology;
        private IConnection connection;
        private IModel channel;

        public DefaultPublisher(IConnectionFactory connectionFactory, ITopology topology)
        {
            this.connectionFactory = connectionFactory;
            this.topology = topology;
            connection = null;
            channel = null;
        }



        public async Task PublishAsync(string subscriberName, string topicName, IDictionary<string, string> headers, byte[] message)
        {

            await @lock.WaitAsync().ConfigureAwait(false);
            try
            {

                var exchangeName = topology.GetExchangeName(subscriberName, topicName);
                var routingKey = topology.GetRoutingKey(subscriberName, topicName);

                connection = connection ?? connectionFactory.CreateConnection();
                await connection.EnsureConnectionAsync();

                channel = channel ?? connection.CreateModel();
                channel.ExchangeDeclarePassive(exchangeName);

                var channelProperties = channel.CreateBasicProperties();
                channelProperties.Headers = headers.ToDictionary(p => p.Key, p => (object)p.Value);
                channel.BasicPublish(exchangeName, routingKey, channelProperties, message);
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
            @lock.Dispose();
        }


    }
}