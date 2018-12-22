using FakeItEasy;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NetBus.Bus;
using NetBus.Serializer;
using NetBus.Test.Messages;
using NetBus.TopicResolver;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetBus.Test
{
    [TestClass]
    public class TopicResolverTest
    {
        [BusTopic("TopicName")]
        class MessageWithTopic { }

        class MessageWithoutTopic { }

        [TestMethod]
        public void ShouldResolveTopic_WithAttribute()
        {
            var topicResolver = new DefaultTopicResolver();

            var topic = topicResolver.ResolveTopicName<MessageWithTopic>();

            Assert.AreEqual("TopicName", topic.ToString());
            Assert.AreNotEqual(typeof(MessageWithTopic), topic.ToString());

        }

        [TestMethod]
        public void ShouldResolveTopic_WithoutAttribute()
        {
            var topicResolver = new DefaultTopicResolver();

            var topic = topicResolver.ResolveTopicName<MessageWithoutTopic>();

            Assert.AreEqual(typeof(MessageWithoutTopic).Name, topic.ToString());

        }

    }
}