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
using TheJoyOfCode.QualityTools;
using Rhino.Mocks;
using Ukadc.Diagnostics.Utils.PropertyReaders;

namespace Ukadc.Diagnostics.Tests
{
    [TestClass]
    public class TestExtendedSource
    {
        private ExtendedSource _source;

        private void ConfigureNoConfig()
        {
            _source = new ExtendedSource("no_config");
            _source = new ExtendedSource("no_config", SourceLevels.All);
            _source.Listeners.Add(new InMemoryTraceListener());
            InMemoryTraceListener.TraceObjects.Clear();
        }

        [TestMethod]
        public void TestExtendedSource_Message()
        {
            ConfigureNoConfig();
            using (_source.ProfileOperation("hello"))
            {
                // do nothing
            }

            Assert.AreEqual(2, InMemoryTraceListener.TraceObjects.Count);
            Assert.AreEqual("hello", InMemoryTraceListener.TraceObjects[0].Message);
            Assert.AreEqual("hello", InMemoryTraceListener.TraceObjects[1].Message);
            Assert.AreEqual(TraceEventType.Start, InMemoryTraceListener.TraceObjects[0].TraceEventType);
            Assert.AreEqual(TraceEventType.Stop, InMemoryTraceListener.TraceObjects[1].TraceEventType);
        }

        [TestMethod]
        public void TestExtendedSource_Format()
        {
            ConfigureNoConfig();
            using (_source.ProfileOperation("{0}{1}",1, "test" ))
            {
                // do nothing
            }

            Assert.AreEqual(2, InMemoryTraceListener.TraceObjects.Count);
            Assert.AreEqual("1test", InMemoryTraceListener.TraceObjects[0].Message);
            Assert.AreEqual("1test", InMemoryTraceListener.TraceObjects[1].Message);
            Assert.AreEqual(TraceEventType.Start, InMemoryTraceListener.TraceObjects[0].TraceEventType);
            Assert.AreEqual(TraceEventType.Stop, InMemoryTraceListener.TraceObjects[1].TraceEventType);
        }

        [TestMethod]
        public void TestExtendedSource_Data()
        {
            object obj = new object();

            ConfigureNoConfig();
            using (_source.ProfileOperation(obj))
            {
                // do nothing
            }

            Assert.AreEqual(2, InMemoryTraceListener.TraceObjects.Count);
            Assert.IsTrue(Object.ReferenceEquals(obj, InMemoryTraceListener.TraceObjects[0].Data[0]));
            Assert.IsTrue(Object.ReferenceEquals(obj, InMemoryTraceListener.TraceObjects[1].Data[0]));
            Assert.AreEqual(TraceEventType.Start, InMemoryTraceListener.TraceObjects[0].TraceEventType);
            Assert.AreEqual(TraceEventType.Stop, InMemoryTraceListener.TraceObjects[1].TraceEventType);
        }

        [TestMethod]
        public void TestExtendedSource_DataArray()
        {
            ConfigureNoConfig();
            using (_source.ProfileOperation(1,2,3))
            {
                // do nothing
            }

            Assert.AreEqual(2, InMemoryTraceListener.TraceObjects.Count);
            Assert.AreEqual(1, InMemoryTraceListener.TraceObjects[0].Data[0]);
            Assert.AreEqual(2, InMemoryTraceListener.TraceObjects[1].Data[1]);
            Assert.AreEqual(TraceEventType.Start, InMemoryTraceListener.TraceObjects[0].TraceEventType);
            Assert.AreEqual(TraceEventType.Stop, InMemoryTraceListener.TraceObjects[1].TraceEventType);
        }
    }
}
