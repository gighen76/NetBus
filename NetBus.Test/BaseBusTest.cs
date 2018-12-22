using FakeItEasy;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NetBus.Bus;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetBus.Test
{
    [TestClass]
    public class BaseBusTest
    {
        class MockBaseBus : BaseBus
        {

            public MockBaseBus(IBusConfiguration configuration) : base(configuration) { }

            public override Task PublishAsync(string topicName, byte[] message)
            {
                throw new NotImplementedException();
            }

            public override Task SubscribeAsync(string topicName)
            {
                throw new NotImplementedException();
            }
        }


        [TestMethod]
        public void ShoudThrowException_WhenBusConfiguraionIsNull()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
            {
                var baseBus = new MockBaseBus(null);
            });
        }

        [TestMethod]
        public void ShoudReturnSubscriberName_FromIBusConfiguration()
        {
            var subscriberName = "subscriberName";
            var busConfiguration = A.Fake<IBusConfiguration>();
            busConfiguration.SubscriberName = subscriberName;
            var baseBus = A.Fake<BaseBus>(options =>
            {
                options.WithArgumentsForConstructor(new[] { busConfiguration });
            });

            Assert.AreEqual(subscriberName, baseBus.SubscriberName);
        }

    }
}