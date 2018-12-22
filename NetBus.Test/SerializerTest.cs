using FakeItEasy;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NetBus.Bus;
using NetBus.Serializer;
using NetBus.Test.Messages;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetBus.Test
{
    [TestClass]
    public class SerializerTest
    {


        [TestMethod]
        public void ShoudSerializeAndDeserialize_SameObject()
        {

            var serializer = new DefaultSerializer();

            var message = new SerializeMessageTest();

            var deserializedMessage = serializer.Deserialize<SerializeMessageTest>(serializer.Serialize(message));

            Assert.AreEqual(message, deserializedMessage);
        }

        [TestMethod]
        public void ShoudSerializeAndDeserialize_SameValorizedObject()
        {

            var serializer = new DefaultSerializer();

            var message = new SerializeMessageTest
            {
                Decimal = 5m,
                Guid = Guid.NewGuid(),
                Id = new Random().Next(),
                String = "test",
                OptionalDecimal = 10m,
                OptionalGuid = Guid.NewGuid(),
                OptionalId = 3
            };

            var deserializedMessage = serializer.Deserialize<SerializeMessageTest>(serializer.Serialize(message));

            Assert.AreEqual(message, deserializedMessage);
        }

        [TestMethod]
        public void ShoudSerializeAndDeserialize_SimilarObject()
        {
            var serializer = new DefaultSerializer();
            var message = new SerializeMessageTest
            {
                Decimal = 5m,
                Guid = Guid.NewGuid(),
                Id = new Random().Next(),
                String = "test",
                OptionalDecimal = null,
                OptionalGuid = Guid.NewGuid(),
                OptionalId = 3
            };

            var deserializedMessage = serializer.Deserialize<DeserializeMessageTest>(serializer.Serialize(message));

            Assert.AreEqual(message.Id, deserializedMessage.Id);
            Assert.AreEqual(message.OptionalId, deserializedMessage.OptionalId);
            Assert.AreEqual(new Guid(), deserializedMessage.NotSerializedGuid);
            Assert.AreEqual(null, deserializedMessage.NotSerializedString);
            Assert.AreEqual(0m, deserializedMessage.OptionalDecimal);

        }

        [TestMethod]
        public void ShoudDeserializeBusEvent_WithSameId()
        {
            var serializer = new DefaultSerializer();

            var busEvent = new BusEvent<TestMessage>(new TestMessage { });
            var deserializedBusEvent = serializer.Deserialize<BusEvent<TestMessage>>(serializer.Serialize(busEvent));

            Assert.AreEqual(busEvent.Id, deserializedBusEvent.Id);

        }


    }
}