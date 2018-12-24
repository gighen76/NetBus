using FakeItEasy;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NetBus.Bus;
using NetBus.MockBus;
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

        [TestMethod]
        public void ShoudThrowException_WhenBusConfigurationIsNull()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
            {
                var bus = new MockBus.MockBus(null);
            });
        }


        [TestMethod]
        public void ShoudReturnSubscriberName_FromIBusConfiguration()
        {
            var subscriberName = "subscriberName";
            var application = new BusApplication(subscriberName);
            
            var busConfiguration = A.Fake<IBusConfiguration>();
            busConfiguration.Application = application;
            var baseBus = A.Fake<BaseBus>(options =>
            {
                options.WithArgumentsForConstructor(new[] { busConfiguration });
            });

            Assert.AreEqual(subscriberName, baseBus.Application.Name);
        }

        [TestMethod]
        public async Task MustSubscribeTwoTopics()
        {

            var bus = new MockBus.MockBus(new MockBusConfiguration());

            await bus.SubscribeAsync(new BusTopic("a"));
            await bus.SubscribeAsync(new BusTopic("b"));

            Assert.AreEqual(bus.SubscribedTopics.Count(), 2);

        }

        [TestMethod]
        public async Task MustSubscribeOneTopics()
        {

            var bus = new MockBus.MockBus(new MockBusConfiguration());

            await bus.SubscribeAsync(new BusTopic("a"));
            await bus.SubscribeAsync(new BusTopic("a"));

            Assert.AreEqual(bus.SubscribedTopics.Count(), 1);

        }


    }
}