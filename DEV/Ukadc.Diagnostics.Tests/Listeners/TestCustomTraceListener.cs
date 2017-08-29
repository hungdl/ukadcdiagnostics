// THIS CODE AND INFORMATION ARE PROVIDED AS IS WITHOUT WARRANTY OF ANY
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
using System;
using System.Text;
using System.Collections.Generic;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using Ukadc.Diagnostics.Listeners;
using Ukadc.Diagnostics.Filters;
using System.Configuration;
using System.Reflection;
using Ukadc.Diagnostics.Utils;

namespace Ukadc.Diagnostics.Tests.Listeners
{
    [TestClass]
    public class TestCustomTraceListener
    {
        
        private TraceSource _source;
        private TestTraceListener _listener; 

        [TestInitialize]
        public void Setup()
        {
            _source = new TraceSource("TestSource");
            _listener = new TestTraceListener();
            _listener.Filter = new SourceFilter("TestSource");
            _source.Listeners.Add(_listener);
            _source.Switch = new SourceSwitch("test") { Level = SourceLevels.All };
        }

        [TestMethod]
        public void WriteSimpleMessage()
        {
            _source.TraceInformation("Basic test");
            Assert.AreEqual("Basic test", _listener.Messages[0]);
        }

        [TestMethod]
        public void WriteSingleDataItem()
        {
            DateTime data = DateTime.Now;
            _source.TraceData(TraceEventType.Critical, 0, data);

            Assert.AreEqual(data.ToString(), _listener.Messages[0]);
        }

        [TestMethod]
        public void WriteNullDataItem()
        {
            DateTime data = DateTime.Now;
            _source.TraceData(TraceEventType.Critical, 0, null);

            Assert.AreEqual(null, _listener.Messages[0]);
        }

        [TestMethod]
        public void WriteEmptyObjectArray()
        {
            object[] data = new object[0];
            _source.TraceData(TraceEventType.Critical, 0, data);

            Assert.AreEqual(string.Empty, _listener.Messages[0]);
        }

        [TestMethod]
        public void WriteMultipleDataItems()
        {
            DateTime now = DateTime.Now;
            DateTime today = DateTime.Today;
            _source.TraceData(TraceEventType.Critical, 0, now, today);

            Assert.AreEqual(string.Format("{0}|{1}", now, today), _listener.Messages[0]);
        }

        [TestMethod]
        public void WriteStringFormat()
        {
            DateTime now = DateTime.Now;
            _source.TraceEvent(TraceEventType.Information, 0, "Test:{0}", now );

            Assert.AreEqual(string.Format("Test:{0}", now), _listener.Messages[0]);
        }

        [TestMethod]
        public void TwoListenersOneFails()
        {
            TestTraceListener ttl = new TestTraceListener();

            TraceSource source = new TraceSource("test", SourceLevels.All);
            source.Listeners.Add(new FailingTraceListener());
            source.Listeners.Add(ttl);
            source.TraceData(TraceEventType.Information, 0, "boo!");
            source.TraceEvent(TraceEventType.Information, 0, "Hello!");
            Assert.AreEqual("boo!", ttl.Messages[0]);
        }

        [TestMethod]
        public void WriteWithFilter()
        {
            _listener.Filter = new SourceFilter("OtherSource");
            _source.TraceInformation("Basic test");
            _source.TraceData(TraceEventType.Critical, 0, new object());
            _source.TraceEvent(TraceEventType.Error, 0, "boo!");
            Assert.AreEqual(0, _listener.Messages.Count);
        }

        [TestMethod]
        public void TestInternalLogger()
        {
            MockInternalLogger mil = new MockInternalLogger();
            using (DefaultServiceLocator.Instance.OverrideType<IInternalLogger>(mil))
            {
                FailingTraceListener ftl = new FailingTraceListener();
                ftl.TraceData(null, null, TraceEventType.Error, 0, null);
            }

            Assert.AreEqual(1, mil.Exceptions.Count);
            Assert.AreEqual(FailingTraceListener.ERROR_MESSAGE, mil.Exceptions[0].Message);
            Assert.AreEqual(0, mil.Messages.Count);
        }
    }

    public class MockInternalLogger : IInternalLogger
    {
        public readonly List<Exception> Exceptions = new List<Exception>();
        public readonly List<string> Messages = new List<string>();

        public void LogException(Exception exc)
        {
            Exceptions.Add(exc);
        }

        public void LogInformation(string message)
        {
            Messages.Add(message);
        }
    }


    public class FailingTraceListener : CustomTraceListener
    {
        public const string ERROR_MESSAGE = "This listener doesn't work!";

        public FailingTraceListener()
            : base("FailingTraceListener")
        {
        }

        protected override void TraceEventCore(TraceEventCache eventCache, string source, TraceEventType eventType, int id, string message)
        {
            throw new Exception(ERROR_MESSAGE);
        }

        protected override void TraceDataCore(TraceEventCache eventCache, string source, TraceEventType eventType, int id, params object[] data)
        {
            throw new Exception(ERROR_MESSAGE);
        }
    }

    public class TestTraceListener : CustomTraceListener
    {
        public TestTraceListener() : base("TestTraceListener")
        {
            Messages = new List<string>();
        }

        public List<string> Messages { get; set; }

        protected override void TraceEventCore(TraceEventCache eventCache, string source, TraceEventType eventType, int id, string message)
        {
            Messages.Add(message);
        }

        protected override void TraceDataCore(TraceEventCache eventCache, string source, TraceEventType eventType, int id, params object[] data)
        {
            Messages.Add(StringFormatter.FormatData(data));
        }
    }
}
