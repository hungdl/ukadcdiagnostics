using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ukadc.Diagnostics.Listeners;
using System.Diagnostics;
using Rhino.Mocks;
using System.Net.Mail;
using Ukadc.Diagnostics.Utils.PropertyReaders;
using System.Configuration;
using Ukadc.Diagnostics.Utils;

namespace Ukadc.Diagnostics.Tests.Listeners
{
    [TestClass]
    public class TestSmtpTraceListener
    {
        [TestMethod]
        public void TestSendViaSource_NoConfig()
        {
            MockRepository mocks = new MockRepository();
            ISmtpService mockSmtpService = mocks.CreateMock<ISmtpService>();

            using (mocks.Record())
            using (mocks.Ordered())
            {
                 mockSmtpService.Initialize("host", 10,  "username", "password");
                 mockSmtpService.SendMessage("from@address.com", "to@address.com", "Error", "6, test");
                 mockSmtpService.SendMessage("from@address.com", "to@address.com", "Critical", "0, 7|boo");
            }

            SmtpTraceListener listener = new SmtpTraceListener(
                DefaultServiceLocator.GetService<IPropertyReaderFactory>(),
                mockSmtpService, 
                "host", 
                10,
                "username",
                "password",
                "to@address.com", 
                "from@address.com", 
                "{EventType}", 
                "{Id}, {Message}");

            TraceSource source = new TraceSource("test", SourceLevels.All);
            source.Listeners.Add(listener);

            mocks.ReplayAll();
            source.TraceEvent(TraceEventType.Error, 6, "test");
            source.TraceData(TraceEventType.Critical, 0, new object[] { 7, "boo" });

            mocks.VerifyAll();
        }

        [TestMethod]
        [ExpectedException(typeof(ConfigurationErrorsException))]
        public void TestNoMatchingConfigSection()
        {
            new SmtpTraceListener("Nonsense");
        }

        [TestMethod]
        public void TestConfiguration()
        {
            TraceSource source = new TraceSource("source13");
            // kick config into life
            source.TraceEvent(TraceEventType.Verbose, 0);

            SmtpTraceListener listener = source.Listeners[1] as SmtpTraceListener;
            
            Assert.IsNotNull(listener, "listener was expected to be of type SmtpTraceListener");

            Assert.AreEqual(listener.To, "to@address.com");
            Assert.AreEqual(listener.From, "from@address.com");
            Assert.AreEqual(listener.SmtpService.Host, "localhost");
            Assert.AreEqual(listener.SmtpService.Port, 25);
            Assert.AreEqual(listener.SmtpService.Username, "username");
            Assert.AreEqual(listener.SmtpService.Password, "password");
            Assert.IsInstanceOfType(listener.SubjectReader, typeof(LiteralPropertyReader));
            Assert.IsInstanceOfType(listener.BodyReader, typeof(MessagePropertyReader));

        }
    }
}
