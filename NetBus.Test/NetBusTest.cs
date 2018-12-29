using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NetBus.Serializer;
using NetBus.Test.Messages;
using NetBus.MockBus;
using NetBus.TopicResolver;
using System;
using System.Threading.Tasks;
using NetBus.Test.Subscriber;
using NetBus.Subscriber;

namespace NetBus.Test
{
    [TestClass]
    public class NetBusTest
    {


        private NetBus netbus;

        [TestInitialize]
        public void Initialize()
        {

            ServiceCollection sc = new ServiceCollection();
            sc.UseNetBus<MockBusConfiguration>(c =>
            {
                c.Application = new BusApplication("test");
            });

            netbus = sc.BuildServiceProvider().GetRequiredService<NetBus>();

        }

        [TestMethod]
        public async Task TestPubSub()
        {

            bool subscriberCalled = false;
            await netbus.SubscribeAsync(async (BusEvent busEvent, TestMessage message) => {

                subscriberCalled = true;
                await Task.Yield();
            });

            await netbus.PublishAsync(new TestMessage
            {
                Id = 5
            });

            Assert.IsTrue(subscriberCalled);

        }

        [TestMethod]
        public async Task TestPubSubWait()
        {

            int id = 5;
            string messageString = "TEST OK !";

            await netbus.SubscribeAsync(async (BusEvent busEvent, TestMessage message) => {

                await netbus.PublishAsync(new MessageTested
                {
                    Id = message.Id
                }, busEvent);
            });

            await netbus.SubscribeAsync(async (BusEvent busEvent, MessageTested message) => {

                await netbus.PublishAsync(new MessageFinal
                {
                    Id = message.Id,
                    Message = messageString
                }, busEvent);
            });

            var result = await netbus.PublishAndWaitAsync<TestMessage, MessageFinal>(new TestMessage
            {
                Id = id
            }, TimeSpan.FromSeconds(1));

            Assert.AreEqual(id, result.Id);
            Assert.AreEqual(messageString, result.Message);

        }

    }
}
