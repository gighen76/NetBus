using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace NetBus.Test
{

    [TestClass]
    public class BusTopicTest
    {

        [DataTestMethod]
        [DataRow("aaaa_bbbbb")]
        [DataRow("000_aaa_zzz")]
        [DataRow("ClassName")]
        public void ShouldReturnTopic_WithValidTopicName(string topicName)
        {

            var topic = new BusTopic(topicName);

            Assert.AreEqual(topicName, topic.Name);

        }

        [DataTestMethod]
        [DataRow("ààà000aaa")]
        [DataRow("aaa:bbb")]
        [DataRow(" a ")]
        [DataRow("")]
        public void ShouldThrowException_WithInvalidTopicName(string topicName)
        {

            Assert.ThrowsException<ArgumentException>(() => new BusTopic(topicName));

        }

        [TestMethod]
        public void TopicWithSameName_MustBeEquals()
        {
            var topic1 = new BusTopic("name");
            var topic2 = new BusTopic("name");

            Assert.AreEqual(topic1, topic2);
        }

        [TestMethod]
        public void TopicWithSameName_MustHaveSameHashCode()
        {
            var topic1 = new BusTopic("name");
            var topic2 = new BusTopic("name");

            Assert.AreEqual(topic1.GetHashCode(), topic2.GetHashCode());
        }


    }
}