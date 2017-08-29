using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ukadc.Diagnostics.Utils.PropertyReaders;
using System.Diagnostics;

namespace Ukadc.Diagnostics.Tests.Utils
{
    [TestClass]
    public class TestCombinedPropertyReaderFactory
    {
        private ICombinedPropertyReaderFactory _combinedFactory = new CombinedPropertyReaderFactory();
        private IPropertyReaderFactory _readerFactory = new PropertyReaderFactory(true, true);

        [TestMethod]
        public void TestCombinedPropertyReader_FromConfig()
        {
            PropertyReader reader = _combinedFactory.Create("{CombinedToken}", _readerFactory);
            object value;
            Assert.IsTrue(reader.TryGetValue(out value, null, null, TraceEventType.Critical, 24, "message", null, null, null));
            Assert.AreEqual("Critical, message - 24", value);
        }

        [TestMethod]
        public void TestValueTokenReturnsSinglePropertyReader()
        {
            string sample = "{Id}";
            PropertyReader reader = _combinedFactory.Create(sample, _readerFactory);

            Assert.IsInstanceOfType(reader, typeof(IdPropertyReader));
        }

        [TestMethod]
        public void TestLiteralOnlyValueTokenReturnsLiteralReader()
        {
            string sample = "Boo!";
            PropertyReader reader = _combinedFactory.Create(sample, _readerFactory);

            Assert.IsInstanceOfType(reader, typeof(LiteralPropertyReader));
        }

        [TestMethod]
        public void TestNoMatchingTokensReturnsLiteral()
        {
            string sample = "{UtterNonsense}";
            PropertyReader reader = _combinedFactory.Create(sample, _readerFactory);

            object value;
            Assert.IsTrue(reader.TryGetValue(out value, null, null, TraceEventType.Critical, 24, "message", null, null, null));
            Assert.AreEqual(sample, value);
        }

        [TestMethod]
        public void TestTrimValueTokenReturnsSinglePropertyReader()
        {
            // If there's only one token found, we trim the whitespace before deciding to create
            // a CombinedPropertyReader
            string sample = "\r\n{Id}\t ";
            PropertyReader reader = _combinedFactory.Create(sample, _readerFactory);

            Assert.IsInstanceOfType(reader, typeof(IdPropertyReader));
        }

        [TestMethod]
        public void TestCombinedPropertyReader_ParsingString()
        {
            string sample = "{EventType}, {Message} - {Id}, {{Nonsense}} {Non{ActivityId}sen{Id}se}";
            PropertyReader reader = _combinedFactory.Create(sample, _readerFactory);

            object value;
            Trace.CorrelationManager.ActivityId = Guid.Empty;
            Assert.IsTrue(reader.TryGetValue(out value, null, null, TraceEventType.Critical, 24, "message", null, null, null));
            Assert.AreEqual("Critical, message - 24, {{Nonsense}} {Non00000000-0000-0000-0000-000000000000sen24se}", value);
        }

        [TestMethod]
        public void TestCombinedPropertyReader_ParsingEmptyString()
        {
            string sample = "";
            PropertyReader reader = _combinedFactory.Create(sample, _readerFactory);

            object value;

            Assert.IsTrue(reader.TryGetValue(out value, null, null, TraceEventType.Critical, 24, "message", null, null, null));
            Assert.AreEqual("", value);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestPropertyReaderFactoryWithEmtpyValueToken()
        {
            _combinedFactory.Create(null, _readerFactory);
        }
    }
}
