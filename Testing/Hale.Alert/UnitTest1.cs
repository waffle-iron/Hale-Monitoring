using System;
using Hale.Alert;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Hale_Alert_UnitTest
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void Test1()
        {
            Hale.Alert.Slack slack = new Slack();
            slack.SendMessage();
        }
    }
}
