using System;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using XSemmel.Helpers;

namespace XSemmel.Test
{
    [TestClass]
    public class NamedPipeHelperTest
    {
        [TestMethod]
        public void FirstTest()
        {
            string test = "Hallo"+Environment.NewLine+"Welt";
            string result = null;

            const string pipename = "3";
            var t1 = new Thread(() => NamedPipeHelper.Write(pipename, test));
            var t2 = new Thread(() => result = NamedPipeHelper.Read(pipename));
            
            t1.Start();
            t2.Start();

            t1.Join();
            t2.Join();

            Assert.AreEqual(test, result);
        }


        [TestMethod]
        public void TestWithEmptyStream()
        {
            const string test = "";
            string result = null;

            const string pipename = "4";
            var t1 = new Thread(() => NamedPipeHelper.Write(pipename, test));
            var t2 = new Thread(() => result = NamedPipeHelper.Read(pipename));

            t1.Start();
            t2.Start();

            t1.Join();
            t2.Join();

            Assert.AreEqual(test, result);
        }

    }
}
