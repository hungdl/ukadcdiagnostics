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

namespace Ukadc.Diagnostics.Tests.Listeners
{

    [TestClass]
    public class TestInMemoryTraceListener
    {
        private TraceSource _source;

        [TestInitialize]
        public void Setup()
        {
            _source = new TraceSource("TestSource");
            _source.Listeners.Add(new InMemoryTraceListener());
            _source.Switch = new SourceSwitch("test") { Level = SourceLevels.All };
            
            InMemoryTraceListener.TraceObjects.Clear();
        }

        [TestMethod]
        public void WriteBasicMessage()
        {
            _source.TraceInformation("Basic test");
            _source.TraceEvent(TraceEventType.Information, 0, "{0}{1}", 1, "test");
            Assert.AreEqual("Basic test", InMemoryTraceListener.TraceObjects[0].Message);
            Assert.AreEqual("1test", InMemoryTraceListener.TraceObjects[1].Message);
        }

        [TestMethod]
        public void WriteDataObject()
        {
            DateTime data = DateTime.Now;
            _source.TraceData(TraceEventType.Critical, 0, data);

            Assert.AreEqual(data, InMemoryTraceListener.TraceObjects[0].Data[0]);
        }

// only run in Debug, can't use [Conditional] as called via reflection
#if DEBUG
        [TestMethod]
        // No point running this test in Release
        public void CalledViaDebug()
        {
            Debug.Listeners.Add(new InMemoryTraceListener());
            Debug.Write("Hello Morgan");
            Assert.AreEqual("Hello Morgan", InMemoryTraceListener.TraceObjects[0].Message);
        }
#endif

        [TestMethod]
        public void CalledViaTrace()
        {
            Trace.Listeners.Add(new InMemoryTraceListener());
            Trace.WriteLine("Goodbye Morgan");

            Assert.AreEqual("Goodbye Morgan", InMemoryTraceListener.TraceObjects[0].Message);
        }
    }
}
