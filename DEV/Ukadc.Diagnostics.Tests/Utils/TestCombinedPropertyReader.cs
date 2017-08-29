// THIS CODE AND INFORMATION ARE PROVIDED AS IS WITHOUT WARRANTY OF ANY
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ukadc.Diagnostics.Filters;
using Ukadc.Diagnostics.Utils;
using Ukadc.Diagnostics.Utils.PropertyReaders;
using System.Diagnostics;
using Ukadc.Diagnostics.Configuration;
using System.Text.RegularExpressions;

namespace Ukadc.Diagnostics.Tests.Utils
{
    [TestClass]
    public class TestCombinedPropertyReader
    {
        [TestMethod]
        public void TestEmptyCombinedPropertyReader()
        {
            CombinedPropertyReader cr = new CombinedPropertyReader();

            object value;
            Assert.IsTrue(cr.TryGetValue(out value, null, null, TraceEventType.Critical, 24, null, null, null, null));
            Assert.AreEqual("", value);
        }

        [TestMethod]
        public void TestCombinedPropertyReaderWithLiterals()
        {
            CombinedPropertyReader cr = new CombinedPropertyReader();
            cr.PropertyReaders.Add(new LiteralPropertyReader("Hello"));
            cr.PropertyReaders.Add(new LiteralPropertyReader(" "));
            cr.PropertyReaders.Add(new LiteralPropertyReader("Mum"));

            object value;
            Assert.IsTrue(cr.TryGetValue(out value, null, null, TraceEventType.Critical, 0, null, null, null, null));
            Assert.AreEqual("Hello Mum", value);
        }

        [TestMethod]
        public void TestCombinedPropertyReaderWithData()
        {
            CombinedPropertyReader cr = new CombinedPropertyReader();
            cr.PropertyReaders.Add(new MessagePropertyReader());
            cr.PropertyReaders.Add(new LiteralPropertyReader(", "));
            cr.PropertyReaders.Add(new IdPropertyReader());
            cr.PropertyReaders.Add(new LiteralPropertyReader(" - "));
            cr.PropertyReaders.Add(new DateTimePropertyReader());

            // Note - because the cache is null, the DateTimePropertyReader will fail to read and return nothing
            object value;
            TraceEventCache cache = new TraceEventCache();
            Assert.IsTrue(cr.TryGetValue(out value, cache, null, TraceEventType.Critical, 24, "message", null, null, null));
            Assert.AreEqual("message, 24 - " + cache.DateTime.ToString(), value);
        }
    }
}
