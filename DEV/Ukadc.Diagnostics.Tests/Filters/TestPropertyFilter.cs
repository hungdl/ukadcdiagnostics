// THIS CODE AND INFORMATION ARE PROVIDED AS IS WITHOUT WARRANTY OF ANY
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ukadc.Diagnostics.Filters;
using System.Diagnostics;
using Ukadc.Diagnostics.Listeners;
using Ukadc.Diagnostics.Utils;
using System.Configuration;
using Ukadc.Diagnostics.Utils.PropertyReaders;
using System.Threading;

namespace Ukadc.Diagnostics.Tests.Filters
{
    [TestClass]
    public class TestPropertyFilter
    {
        private TraceSource _source;
        private TraceListener _listener;

        [TestInitialize]
        public void Setup()
        {
            InMemoryTraceListener.TraceObjects.Clear();
        }

        private void ConfigureNoConfigSource()
        {
            _source = new TraceSource("NoConfigSource", SourceLevels.All);
            _listener = new InMemoryTraceListener();
            _source.Listeners.Add(_listener);
        }

        [TestMethod]
        public void TestProcessIdPropertyReader_NoConfig()
        {
            ConfigureNoConfigSource();

            int _currentProcessId = System.Diagnostics.Process.GetCurrentProcess().Id;
            // create a process id property Reader with the current Process' Id.
            ProcessIdPropertyReader pipp = new ProcessIdPropertyReader();
            PropertyFilter filter = new PropertyFilter(_currentProcessId.ToString(), Operation.Equals, pipp);
            _listener.Filter = filter;

            Assert.AreEqual(0, InMemoryTraceListener.TraceObjects.Count);

            _source.TraceEvent(TraceEventType.Critical, 0);
            Assert.AreEqual(1, InMemoryTraceListener.TraceObjects.Count);

            // change the filter to have a different numeric value (i.e. NOT the current process id);
            pipp = new ProcessIdPropertyReader();
            filter = new PropertyFilter("0", Operation.Equals, pipp);
            _listener.Filter = filter;
            _source.TraceEvent(TraceEventType.Critical, 0);
            Assert.AreEqual(1, InMemoryTraceListener.TraceObjects.Count);
        }

        [TestMethod]
        public void TestDateTimePropertyReader_NoConfig()
        {
            ConfigureNoConfigSource();

            DateTimePropertyReader dtpp = new DateTimePropertyReader();
            // allow only events after now :-)
            PropertyFilter filter = new PropertyFilter(DateTime.Now.ToUniversalTime().ToString(), Operation.GreaterThan, dtpp);
            _listener.Filter = filter;

            Assert.AreEqual(0, InMemoryTraceListener.TraceObjects.Count);

            _source.TraceEvent(TraceEventType.Critical, 0);
            Assert.AreEqual(1, InMemoryTraceListener.TraceObjects.Count);

            // chance filter to allow only events before now :-)
            dtpp = new DateTimePropertyReader();
            filter = new PropertyFilter(DateTime.Now.ToUniversalTime().ToString(), Operation.LessThan, dtpp);
            _listener.Filter = filter;
            _source.TraceEvent(TraceEventType.Critical, 0);
            Assert.AreEqual(1, InMemoryTraceListener.TraceObjects.Count);
        }

        [TestMethod]
        public void TestEventTypePropertyReader_NoConfig()
        {
            ConfigureNoConfigSource();

            EventTypePropertyReader etpp = new EventTypePropertyReader();

            PropertyFilter filter = new PropertyFilter("Critical", Operation.GreaterThan, etpp);
            _listener.Filter = filter;

            Assert.AreEqual(0, InMemoryTraceListener.TraceObjects.Count);

            _source.TraceEvent(TraceEventType.Warning, 0);
            Assert.AreEqual(1, InMemoryTraceListener.TraceObjects.Count);

            etpp = new EventTypePropertyReader();
            filter = new PropertyFilter("Warning", Operation.GreaterThan, etpp);
            _listener.Filter = filter;
            _source.TraceEvent(TraceEventType.Warning, 0);
            Assert.AreEqual(1, InMemoryTraceListener.TraceObjects.Count);
        }

        [TestMethod]
        public void TestSourcePropertyReader_NoConfig()
        {
            ConfigureNoConfigSource();

            SourcePropertyReader spp = new SourcePropertyReader();
        
            PropertyFilter filter = new PropertyFilter(_source.Name, Operation.Equals, spp);
            _listener.Filter = filter;

            Assert.AreEqual(0, InMemoryTraceListener.TraceObjects.Count);

            _source.TraceEvent(TraceEventType.Warning, 0);
            Assert.AreEqual(1, InMemoryTraceListener.TraceObjects.Count);

            spp = new SourcePropertyReader();
            filter = new PropertyFilter("nonsense", Operation.Equals, spp);
            _listener.Filter = filter;
            _source.TraceEvent(TraceEventType.Warning, 0);
            Assert.AreEqual(1, InMemoryTraceListener.TraceObjects.Count);
        }

        [TestMethod]
        public void TestCallstackPropertyReader_NoConfig()
        {
            ConfigureNoConfigSource();

            CallstackPropertyReader cpp = new CallstackPropertyReader();
            // allow only events after now :-)
            PropertyFilter filter = new PropertyFilter("TestCallstackPropertyReader_NoConfig", Operation.Contains, cpp);
            _listener.Filter = filter;

            Assert.AreEqual(0, InMemoryTraceListener.TraceObjects.Count);

            _source.TraceEvent(TraceEventType.Critical, 0);
            Assert.AreEqual(1, InMemoryTraceListener.TraceObjects.Count);

            // chance filter to allow only events before now :-)
            cpp = new CallstackPropertyReader();
            filter = new PropertyFilter("Nonsense", Operation.Contains, cpp);
            _listener.Filter = filter;
            _source.TraceEvent(TraceEventType.Critical, 0);
            Assert.AreEqual(1, InMemoryTraceListener.TraceObjects.Count);
        }

        [TestMethod]
        public void TestThreadIdPropertyReader_NoConfig()
        {
            ConfigureNoConfigSource();

            ThreadIdPropertyReader tipp = new ThreadIdPropertyReader();
            // allow only events after now :-)
            PropertyFilter filter = new PropertyFilter(Thread.CurrentThread.ManagedThreadId.ToString(), Operation.Equals, tipp);
            _listener.Filter = filter;

            Assert.AreEqual(0, InMemoryTraceListener.TraceObjects.Count);

            _source.TraceEvent(TraceEventType.Critical, 0);
            Assert.AreEqual(1, InMemoryTraceListener.TraceObjects.Count);

            // chance filter to allow only events before now :-)
            tipp = new ThreadIdPropertyReader();
            filter = new PropertyFilter("Nonsense", Operation.Contains, tipp);
            _listener.Filter = filter;
            _source.TraceEvent(TraceEventType.Critical, 0);
            Assert.AreEqual(1, InMemoryTraceListener.TraceObjects.Count);
        }

        [TestMethod]
        public void TestMessagePropertyReader_WithConfig()
        {
            TraceSource source = new TraceSource("source4");
            Assert.AreEqual(0, InMemoryTraceListener.TraceObjects.Count);
            source.TraceEvent(TraceEventType.Critical, 0, "test");
            Assert.AreEqual(0, InMemoryTraceListener.TraceObjects.Count);
            source.TraceEvent(TraceEventType.Critical, 0, "must contain expected value!");
            Assert.AreEqual(1, InMemoryTraceListener.TraceObjects.Count);
        }

        [TestMethod]
        public void TestIdPropertyReader_WithConfig()
        {
            TraceSource source = new TraceSource("source7");
            Assert.AreEqual(0, InMemoryTraceListener.TraceObjects.Count);
            source.TraceEvent(TraceEventType.Critical, 0, "test");
            Assert.AreEqual(0, InMemoryTraceListener.TraceObjects.Count);
            source.TraceEvent(TraceEventType.Critical, 1, "must contain expected value!");
            Assert.AreEqual(1, InMemoryTraceListener.TraceObjects.Count);
            source.TraceEvent(TraceEventType.Critical, 2, "must contain expected value!");
            Assert.AreEqual(2, InMemoryTraceListener.TraceObjects.Count);
        }

        [TestMethod]
        [ExpectedException(typeof(ConfigurationErrorsException), "No propertyFilter with the name 'nonsenseReference' could be found")]
        public void TestInvalidPropertyFilterRef_WithConfig()
        {
            TraceSource source = new TraceSource("source10");
            source.TraceEvent(TraceEventType.Critical, 0);
        }

        [TestMethod]
        [ExpectedException(typeof(ConfigurationErrorsException))]
        public void TestInvalidConfig_WithConfig()
        {
            TraceSource source = new TraceSource("source5");
            source.TraceEvent(TraceEventType.Critical, 0, "test");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestPropertyFilterNullProperytReader()
        {
            new PropertyFilter("", Operation.Equals, null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestPropertyFilterEmptyPropertyFilterName()
        {
            new PropertyFilter("");
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TestParseValueInvalidType()
        {
            PropertyFilter filter = new PropertyFilter("str", Operation.Equals, new ObjectPropertyReader());
        }

        public class ObjectPropertyReader : PropertyReader
        {
            public override Type PropertyType
            {
                get { return typeof(object); }
            }

            public override IComparator Comparator
            {
                get { return new StringComparator(); }
            }

            public override bool TryGetValue(out object value, TraceEventCache cache, string source, TraceEventType eventType, int id, string formatOrMessage, object[] data)
            {
                throw new NotImplementedException();
            }
        }
    }
}
