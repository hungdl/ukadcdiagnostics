using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using Ukadc.Diagnostics.Filters;
using Ukadc.Diagnostics.Listeners;
using Ukadc.Diagnostics.Utils;
using Ukadc.Diagnostics.Utils.PropertyReaders;

namespace Ukadc.Diagnostics.Tests.Filters
{
    [TestClass]
    public class TestDynamicPropertyReader
    {
        private TraceSource _source;
        private TraceListener _listener;

        [TestInitialize]
        public void Setup()
        {
            _source = new TraceSource("Bananas", SourceLevels.All);
            _listener = new InMemoryTraceListener();
            _source.Listeners.Add(_listener);
            InMemoryTraceListener.TraceObjects.Clear();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestNullSourceType()
        {
            DynamicPropertyReader dpp = new DynamicPropertyReader(null, null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestNullPropertyName()
        {
            DynamicPropertyReader dpp = new DynamicPropertyReader("Bob", null);
        }

        [TestMethod]
        public void TestReaderWithIntValue()
        {
            DynamicPropertyReader dpp = new DynamicPropertyReader(
                "Ukadc.Diagnostics.Tests.Filters.DummyDataObject, Ukadc.Diagnostics.Tests", "IntProperty");
            PropertyFilter filter = new PropertyFilter("3", Operation.GreaterThan, dpp);
            _listener.Filter = filter;

            Assert.AreEqual(0, InMemoryTraceListener.TraceObjects.Count);
            _source.TraceData(TraceEventType.Critical, 0, new DummyDataObject() { IntProperty = 3 });
            Assert.AreEqual(0, InMemoryTraceListener.TraceObjects.Count);
            _source.TraceData(TraceEventType.Critical, 0, new DummyDataObject() { IntProperty = 4});
            Assert.AreEqual(1, InMemoryTraceListener.TraceObjects.Count);
        }

        [TestMethod]
        public void TestReaderWithStringValue()
        {
            DynamicPropertyReader dpp = new DynamicPropertyReader(
                "Ukadc.Diagnostics.Tests.Filters.DummyDataObject, Ukadc.Diagnostics.Tests", "StringProperty");
    
            PropertyFilter filter = new PropertyFilter("007", Operation.Contains, dpp);
            _listener.Filter = filter;

            Assert.AreEqual(0, InMemoryTraceListener.TraceObjects.Count);
            _source.TraceData(TraceEventType.Critical, 0, new DummyDataObject() { StringProperty = "70089" });
            Assert.AreEqual(0, InMemoryTraceListener.TraceObjects.Count);
            _source.TraceData(TraceEventType.Critical, 0, new DummyDataObject() { StringProperty = "000070" });
            Assert.AreEqual(1, InMemoryTraceListener.TraceObjects.Count);
        }

        [TestMethod]
        public void TestReaderWithDateTimeValue()
        {
            DynamicPropertyReader dpp = new DynamicPropertyReader(
                "Ukadc.Diagnostics.Tests.Filters.DummyDataObject, Ukadc.Diagnostics.Tests", "DateTimeProperty");
            PropertyFilter filter = new PropertyFilter("2008-12-31", Operation.GreaterThanOrEqualTo, dpp);
            _listener.Filter = filter;

            Assert.AreEqual(0, InMemoryTraceListener.TraceObjects.Count);
            _source.TraceData(TraceEventType.Critical, 0, new DummyDataObject() { DateTimeProperty = new DateTime(2007, 12, 31) });
            Assert.AreEqual(0, InMemoryTraceListener.TraceObjects.Count);
            _source.TraceData(TraceEventType.Critical, 0, new DummyDataObject() { DateTimeProperty = new DateTime(2008, 12, 31) });
            Assert.AreEqual(1, InMemoryTraceListener.TraceObjects.Count);
        }

        [TestMethod]
        public void TestDynamicPropertyReader_WithConfig()
        {
            TraceSource source = new TraceSource("source6");
            Assert.AreEqual(0, InMemoryTraceListener.TraceObjects.Count);
            source.TraceData(TraceEventType.Critical, 0, new DummyDataObject() { StringProperty = "not going to match" });
            Assert.AreEqual(0, InMemoryTraceListener.TraceObjects.Count);
            source.TraceData(TraceEventType.Critical, 0, new DummyDataObject() { StringProperty = "this should match" });
            Assert.AreEqual(1, InMemoryTraceListener.TraceObjects.Count);
        }


        [TestMethod]
        public void TestDefaultEvaluationWhenNoMatch_WithConfig()
        {
            TraceSource source = new TraceSource("source6");
            Assert.AreEqual(0, InMemoryTraceListener.TraceObjects.Count);
            source.TraceData(TraceEventType.Critical, 0, null);
            Assert.AreEqual(1, InMemoryTraceListener.TraceObjects.Count);
            source.TraceEvent(TraceEventType.Critical, 0);
            Assert.AreEqual(2, InMemoryTraceListener.TraceObjects.Count);
        }

        [TestMethod]
        public void TestConfiguredDefaultEvaluationWhenNoMatch_WithConfig()
        {
            TraceSource source = new TraceSource("source9");
            Assert.AreEqual(0, InMemoryTraceListener.TraceObjects.Count);
            source.TraceData(TraceEventType.Critical, 0, null);
            Assert.AreEqual(0, InMemoryTraceListener.TraceObjects.Count);
            source.TraceEvent(TraceEventType.Critical, 0);
            Assert.AreEqual(0, InMemoryTraceListener.TraceObjects.Count);
            source.TraceData(TraceEventType.Critical, 0, new DummyDataObject() { StringProperty = "this should match" });
            Assert.AreEqual(1, InMemoryTraceListener.TraceObjects.Count);
        }

        [TestMethod]
        public void TestWritingArrayOfData()
        {
            DynamicPropertyReader dpp = new DynamicPropertyReader(
                "Ukadc.Diagnostics.Tests.Filters.DummyDataObject, Ukadc.Diagnostics.Tests", "DateTimeProperty");
            PropertyFilter filter = new PropertyFilter("2008-12-31", Operation.GreaterThanOrEqualTo, dpp);
            _listener.Filter = filter;

            Assert.AreEqual(0, InMemoryTraceListener.TraceObjects.Count);
            _source.TraceData(TraceEventType.Critical, 0, new object[] { new object(), new DummyDataObject() { DateTimeProperty = new DateTime(2007, 12, 31) }});
            Assert.AreEqual(0, InMemoryTraceListener.TraceObjects.Count);
            _source.TraceData(TraceEventType.Critical, 0, new object[] { new object(), new DummyDataObject() { DateTimeProperty = new DateTime(2008, 12, 31) }});
            Assert.AreEqual(1, InMemoryTraceListener.TraceObjects.Count);
        }

        [TestMethod]
        public void TestWritingArrayOfNotMatchingData()
        {
            DynamicPropertyReader dpp = new DynamicPropertyReader(
                "Ukadc.Diagnostics.Tests.Filters.DummyDataObject, Ukadc.Diagnostics.Tests", "DateTimeProperty");
            PropertyFilter filter = new PropertyFilter("2008-12-31", Operation.GreaterThanOrEqualTo, dpp);
            _listener.Filter = filter;

            // default evaluation is true so this should go through...

            Assert.AreEqual(0, InMemoryTraceListener.TraceObjects.Count);
            _source.TraceData(TraceEventType.Critical, 0, new object[] { new object(), new object() });
            Assert.AreEqual(1, InMemoryTraceListener.TraceObjects.Count);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException), "The property 'ObjectProperty' is of type 'Ukadc.Diagnostics.Tests.Filters.DummyDataObject, Ukadc.Diagnostics.Tests' which does not implement IComparable or IConvertible and therefore cannot be evaluated or compared against.")]
        public void TestInvalidPropertyType_WithConfig()
        {
            DynamicPropertyReader Reader = new DynamicPropertyReader(
                "Ukadc.Diagnostics.Tests.Filters.DummyDataObject, Ukadc.Diagnostics.Tests",
                "ObjectProperty");
        }

        [TestMethod]
        [ExpectedException(typeof(TypeLoadException))]
        public void TestNonsenseType()
        {
            DynamicPropertyReader Reader = new DynamicPropertyReader(
                "There.Is.No.Such.Type",
                "ObjectProperty");
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void IncompatibleOperation()
        {
            DynamicPropertyReader dpp = new DynamicPropertyReader(
                "Ukadc.Diagnostics.Tests.Filters.DummyDataObject, Ukadc.Diagnostics.Tests", "DateTimeProperty");
            PropertyFilter filter = new PropertyFilter("2008-12-31", Operation.Contains, dpp);
        }
        
        /// <summary>
        /// This is a very rudimentary test to compare the performance of the (very) complex
        /// DynamicPropertyReader combined with the PropertyFilter against the EventTypeFilter
        /// that is provided OOB with System.Diagnostics. The test will fail if it takes longer than
        /// 110% of the EventTypeFilter run. Note, because this is a very long running test it should probably be
        /// commented out or conditional as part of a server build (not a dev build!!).
        /// </summary>
        //[TestMethod]
        public void TestPerformance()
        {
            int loops = 10000;

            TraceSource source = new TraceSource("notfromconfig", SourceLevels.All);
            TraceListener listener = new InMemoryTraceListener();
            source.Listeners.Add(listener);
            DynamicPropertyReader propertyReader = new DynamicPropertyReader(
                "Ukadc.Diagnostics.Tests.Filters.DummyDataObject, Ukadc.Diagnostics.Tests",
                "IntProperty");
            
            // Run test with a dynamic propertyFilter
            PropertyFilter propertyFilter = new PropertyFilter("1", Operation.Equals, propertyReader);
            long firstRun = StressFilter(loops, source, listener, propertyFilter, loops);

            // run with the event type filter
            EventTypeFilter eventTypeFilter = new EventTypeFilter(SourceLevels.Critical);
            long secondRun = StressFilter(loops, source, listener, eventTypeFilter, loops);

            IdPropertyReader idReader = new IdPropertyReader();
            propertyFilter = new PropertyFilter("1", Operation.Equals, idReader);
            long thirdRun = StressFilter(loops, source, listener, propertyFilter, loops);

            Trace.WriteLine(string.Format(
                "{0} loops: PropertyFilter(Dynamic) {1}ms, PropertyFilter(Id) {3}ms EventTypeFilter {2}ms.",
                loops, firstRun, secondRun, thirdRun));

            Assert.IsTrue(firstRun < secondRun * 1.1, "The PropertyFilter took more than 110% of the time taken by the EventTypeFilter overall");
        }

        private static long StressFilter(int loops, TraceSource source, TraceListener listener, TraceFilter filter, int successfulEvents)
        {
            listener.Filter = filter;
            InMemoryTraceListener.TraceObjects.Clear();

            DummyDataObject dummy1 = new DummyDataObject() { IntProperty = 0 };
            DummyDataObject dummy2 = new DummyDataObject() { IntProperty = 1 };

            // warm up
            source.TraceData(TraceEventType.Error, 0, dummy1);

            Stopwatch watch = new Stopwatch();
            watch.Start();
            for (int i = 0; i < loops; i++)
            {
                source.TraceData(TraceEventType.Error, 0, dummy1);
                source.TraceData(TraceEventType.Critical, 1, dummy2);
            }
            watch.Stop();
            Assert.AreEqual(successfulEvents, InMemoryTraceListener.TraceObjects.Count);
            return watch.ElapsedMilliseconds;
        }
    }

    public class DummyDataObject
    {
        public string StringProperty {get; set;}
        public int IntProperty { get; set;}
        public DateTime DateTimeProperty { get; set;}
        public object ObjectProperty { get; set; }
    }
}
