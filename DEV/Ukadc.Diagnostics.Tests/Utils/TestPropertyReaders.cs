using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ukadc.Diagnostics.Utils.PropertyReaders;
using System.Diagnostics;
using System.Security.Principal;

namespace Ukadc.Diagnostics.Tests.Utils
{
    [TestClass]
    public class TestPropertyReaders
    {
        [TestMethod]
        public void TestLiteralPropertyReader()
        {
            LiteralPropertyReader reader = new LiteralPropertyReader("Hello");
            object value;
            Assert.IsTrue(reader.TryGetValue(out value, null, null, TraceEventType.Critical, 0, null, null, null, null));
            Assert.AreEqual("Hello", value);
        }

        [TestMethod]
        public void TestEmptyLiteralPropertyReader()
        {
            LiteralPropertyReader reader = new LiteralPropertyReader(null);
            object value;
            Assert.IsTrue(reader.TryGetValue(out value, null, null, TraceEventType.Critical, 0, null, null, null, null));
            Assert.AreEqual(null, value);
        }

        [TestMethod]
        public void TestMachineName()
        {
            MachineNamePropertyReader reader = new MachineNamePropertyReader();
            object val;
            Assert.IsTrue(reader.TryGetValue(out val, null, null, TraceEventType.Warning, 0, null, null));

            Assert.AreEqual(Environment.MachineName, val);
        }

        [TestMethod]
        public void TestMessagePropertyReaderWithData()
        {
            PropertyReader reader = new MessagePropertyReader();

            object value;
            reader.TryGetValue(out value, null, null, TraceEventType.Critical, 0, null, new object[] { "wibble" });

            Assert.AreEqual("wibble", value);
        }

        [TestMethod]
        public void TestMessagePropertyReaderWithMultipleData()
        {
            PropertyReader reader = new MessagePropertyReader();

            object value;
            reader.TryGetValue(out value, null, null, TraceEventType.Critical, 0, null, new object[] { "wibble", "wobble" });

            Assert.AreEqual("wibble|wobble", value);
        }

        [TestMethod]
        public void TestMessagePropertyReaderWithNullData()
        {
            PropertyReader reader = new MessagePropertyReader();

            object value;
            reader.TryGetValue(out value, null, null, TraceEventType.Critical, 0, null, null);

            Assert.AreEqual(null, value);
        }

        [TestMethod]
        public void TestDataPropertyToken_FromConfig()
        {
            PropertyReader reader = new PropertyReaderFactory(true, true).Create("{DynamicToken}");

            object value;
            reader.TryGetValue(out value, null, null, TraceEventType.Critical, 0, null, new object[] { TimeSpan.FromMilliseconds(20) });

            Assert.AreEqual(20d, value);
        }

        [TestMethod]
        public void TestProcessIdReader()
        {
            ProcessIdPropertyReader reader = new ProcessIdPropertyReader();

            TraceEventCache cache = new TraceEventCache();

            object value;
            reader.TryGetValue(out value, cache, null, TraceEventType.Error, 0, null, null);
            Assert.AreEqual(cache.ProcessId, value);
            reader.TryGetValue(out value, null, null, TraceEventType.Error, 0, null, null);
            Assert.AreEqual(null, value);
        }

        [TestMethod]
        public void TestThreadIdReader()
        {
            ThreadIdPropertyReader reader = new ThreadIdPropertyReader();

            TraceEventCache cache = new TraceEventCache();

            object value;
            reader.TryGetValue(out value, cache, null, TraceEventType.Error, 0, null, null);
            Assert.AreEqual(cache.ThreadId, value);
            reader.TryGetValue(out value, null, null, TraceEventType.Error, 0, null, null);
            Assert.AreEqual(null, value);
        }

        [TestMethod]
        public void TestCallstackReader()
        {
            CallstackPropertyReader reader = new CallstackPropertyReader();

            TraceEventCache cache = new TraceEventCache();

            object value;
            reader.TryGetValue(out value, cache, null, TraceEventType.Error, 0, null, null);
            Assert.AreEqual(cache.Callstack, value);
            reader.TryGetValue(out value, null, null, TraceEventType.Error, 0, null, null);
            Assert.AreEqual(null, value);
        }

        [TestMethod]
        public void TestDateTimeReader()
        {
            DateTimePropertyReader reader = new DateTimePropertyReader();

            TraceEventCache cache = new TraceEventCache();

            object value;
            reader.TryGetValue(out value, cache, null, TraceEventType.Error, 0, null, null);
            Assert.AreEqual(cache.DateTime, value);
            reader.TryGetValue(out value, null, null, TraceEventType.Error, 0, null, null);
            Assert.AreEqual(null, value);
        }

        [TestMethod]
        public void TestLocalTimeReader()
        {
            LocalTimePropertyReader reader = new LocalTimePropertyReader();

            TraceEventCache cache = new TraceEventCache();

            object value;
            reader.TryGetValue(out value, cache, null, TraceEventType.Error, 0, null, null);
            Assert.AreEqual(cache.DateTime.ToLocalTime(), value);
            reader.TryGetValue(out value, null, null, TraceEventType.Error, 0, null, null);
            Assert.AreEqual(null, value);
        }

        [TestMethod]
        public void TestTimestampReader()
        {
            TimestampPropertyReader reader = new TimestampPropertyReader();

            TraceEventCache cache = new TraceEventCache();

            object value;
            reader.TryGetValue(out value, cache, null, TraceEventType.Error, 0, null, null);
            Assert.AreEqual(cache.Timestamp, value);
            reader.TryGetValue(out value, null, null, TraceEventType.Error, 0, null, null);
            Assert.AreEqual(null, value);
        }

        [TestMethod]
        public void TestProcessNamePropertyReader()
        {
            ProcessNamePropertyReader reader = new ProcessNamePropertyReader();
            object value;
            reader.TryGetValue(out value, null, null, TraceEventType.Error, 0, null, null);

            Assert.AreEqual(Process.GetCurrentProcess().ProcessName, value);
        }

        [TestMethod]
        public void TestPrincipalNamePropertyReader()
        {
            PrincipalNamePropertyReader reader = new PrincipalNamePropertyReader();
            object value;
            reader.TryGetValue(out value, null, null, TraceEventType.Error, 0, null, null);

            Assert.AreEqual(string.Empty, value);
        }

        [TestMethod]
        public void TestWindowsIdentityReader()
        {
            WindowsIdentityPropertyReader reader = new WindowsIdentityPropertyReader();
            object value;
            reader.TryGetValue(out value, null, null, TraceEventType.Error, 0, null, null);

            Assert.AreEqual(WindowsIdentity.GetCurrent().Name, value);
        }

        [TestMethod]
        public void TestDatePartPropertyReaders()
        {
            PropertyReaderFactory factory = new PropertyReaderFactory(false, true);
            PropertyReader reader = factory.CreateCombinedReader("{Year}-{Month}-{Day} {Hour}:{Minute}:{Second}.{Millisecond}");

            TraceEventCache cache = new TraceEventCache();
            
            object value;
            reader.TryGetValue(out value, cache, null, TraceEventType.Information, 0, null, null);
            Assert.AreEqual(cache.DateTime.ToString("yyyy-MM-dd HH:mm:ss.fff"), value);
            reader.TryGetValue(out value, null, null, TraceEventType.Information, 0, null, null);
            Assert.AreEqual("-- ::.", value);
        }

        [TestMethod]
        public void TestFormattedPropertyReaders()
        {
            PropertyReaderFactory factory = new PropertyReaderFactory(false, true);
            PropertyReader reader = factory.Create("{DateTime:yyyy-MM-dd HH:mm:ss.fff}");
            TraceEventCache cache = new TraceEventCache();

            object value;
            reader.TryGetValue(out value, cache, null, TraceEventType.Information, 0, null, null);
            Assert.AreEqual(cache.DateTime.ToString("yyyy-MM-dd HH:mm:ss.fff"), value);
        }

        [TestMethod]
        public void TestAdvancedCombinedFormattedPropertyReaders()
        {
            PropertyReaderFactory factory = new PropertyReaderFactory(false, true);
            PropertyReader reader = factory.CreateCombinedReader("{DateTime:yyyy-MM-dd HH:mm:ss.fff}");
            TraceEventCache cache = new TraceEventCache();

            object value;
            reader.TryGetValue(out value, cache, null, TraceEventType.Information, 0, null, null);
            Assert.AreEqual(cache.DateTime.ToString("yyyy-MM-dd HH:mm:ss.fff"), value);
        }

        [TestMethod]
        public void TestCombinedFormattedPropertyReaders()
        {
            PropertyReaderFactory factory = new PropertyReaderFactory(false, true);
            PropertyReader reader = factory.CreateCombinedReader("{DateTime:yyyy}-{DateTime:MM}-{DateTime:dd} {DateTime:HH}:{DateTime:mm}:{DateTime:ss}.{DateTime:fff}");

            TraceEventCache cache = new TraceEventCache();

            object value;
            reader.TryGetValue(out value, cache, null, TraceEventType.Information, 0, null, null);
            Assert.AreEqual(cache.DateTime.ToString("yyyy-MM-dd HH:mm:ss.fff"), value);
            reader.TryGetValue(out value, null, null, TraceEventType.Information, 0, null, null);
            Assert.AreEqual("-- ::.", value);
        }

        [TestMethod]
        public void TestSubPropertyReader()
        {
            PropertyReaderFactory factory = new PropertyReaderFactory(false, true);
            PropertyReader reader = factory.Create("{Process.ProcessName}");

            object value;
            reader.TryGetValue(out value, null, null, TraceEventType.Information, 0, null, null);
            Assert.AreEqual(Process.GetCurrentProcess().ProcessName, value);
        }
    }
}


