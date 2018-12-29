using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace NetBus.Test
{

    [TestClass]
    public class BusApplicationTest
    {

        [DataTestMethod]
        [DataRow("aaaa_bbbbb")]
        [DataRow("000_aaa_zzz")]
        [DataRow("ClassName")]
        public void ShouldReturnApplication_WithValidName(string applicationName)
        {

            var application = new BusApplication(applicationName);

            Assert.AreEqual(applicationName, application.Name);

        }

        [DataTestMethod]
        [DataRow("ààà000aaa")]
        [DataRow("aaa:bbb")]
        [DataRow(" a ")]
        [DataRow("")]
        public void ShouldThrowException_WithInvalidName(string applicationName)
        {

            Assert.ThrowsException<ArgumentException>(() => new BusApplication(applicationName));

        }

        [TestMethod]
        public void ApplicationWithSameName_MustBeEquals()
        {
            var application1 = new BusApplication("name");
            var application2 = new BusApplication("name");

            Assert.AreEqual(application1, application2);
            Assert.IsTrue(application1 == application2);
        }

        [TestMethod]
        public void ApplicationWithSameName_MustBeNotEquals()
        {
            var application1 = new BusApplication("name1");
            var application2 = new BusApplication("name2");

            Assert.AreNotEqual(application1, application2);
            Assert.IsTrue(application1 != application2);
        }

        [TestMethod]
        public void ApplicationWithSameName_MustHaveSameHashCode()
        {
            var application1 = new BusApplication("name");
            var application2 = new BusApplication("name");

            Assert.AreEqual(application1.GetHashCode(), application2.GetHashCode());
        }


        [TestMethod]
        public void NullApplications_MustBeEquals()
        {
            BusApplication application1 = null;
            BusApplication application2 = null;
            Assert.AreEqual(application1, application2);
            Assert.IsTrue(application1 == application2);

        }


    }
}