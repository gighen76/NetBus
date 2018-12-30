using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace NetBus.Test
{

    [TestClass]
    public class BusEventTest
    {

        [TestMethod]
        public void ShouldReplicate_UsingBusHeaders()
        {

            var message = new byte[] { };
            var topic = new BusTopic("test");

            var parentBusEvent = new BusEvent(message, topic);
            var busEvent = new BusEvent(message, topic, parentBusEvent);
            busEvent.Headers.Add("test", "value");

            var replicatedBusEvent = new BusEvent(busEvent.Message, busEvent.GetBusHeaders());

            Assert.AreEqual(busEvent.Id, replicatedBusEvent.Id);
            Assert.AreEqual(busEvent.ParentId, replicatedBusEvent.ParentId);
            Assert.AreEqual(busEvent.OriginId, replicatedBusEvent.OriginId);
            Assert.IsTrue(busEvent.Headers.All(h => replicatedBusEvent.Headers.ContainsKey(h.Key) && replicatedBusEvent.Headers[h.Key] == h.Value));
        }

        [TestMethod]
        public void ShouldGenerateCorrectChildEvent_FromParent()
        {

            var message = new byte[] { };
            var topic = new BusTopic("test");

            var ancestorBusEvent = new BusEvent(message, topic);
            var parentBusEvent = new BusEvent(message, topic, ancestorBusEvent);
            var busEvent = new BusEvent(message, topic, parentBusEvent);

            Assert.AreNotEqual(parentBusEvent.Id, busEvent.Id);
            Assert.AreEqual(parentBusEvent.Id, busEvent.ParentId);
            Assert.AreEqual(parentBusEvent.OriginId, busEvent.OriginId);
            Assert.AreEqual(ancestorBusEvent.Id, busEvent.OriginId);

        }



    }
}