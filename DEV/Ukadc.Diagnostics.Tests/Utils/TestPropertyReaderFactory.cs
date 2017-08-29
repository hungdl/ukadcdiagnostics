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
using Ukadc.Diagnostics.Utils;
using Ukadc.Diagnostics.Listeners;
using Ukadc.Diagnostics.Utils.PropertyReaders;
using Ukadc.Diagnostics.Configuration;
using System.IO;
using System.Reflection;
using System.Configuration;

namespace Ukadc.Diagnostics.Tests.Filters
{
    [TestClass]
    public class TestPropertyReaderFactory
    {
        private IPropertyReaderFactory _readerFactory;

        [TestInitialize]
        public void Setup()
        {
            InMemoryTraceListener.TraceObjects.Clear();
            _readerFactory = new PropertyReaderFactory(true, true);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestValidateToken()
        {
            _readerFactory.Create("");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestValidateDataPropertyElement()
        {
            _readerFactory.Create((DataPropertyElement)null);
        }

        [TestMethod]
        public void TestMessageToken()
        {
            PropertyReader Reader = _readerFactory.Create("{Message}");
            Assert.AreEqual(Reader.GetType(), typeof(MessagePropertyReader));
        }

        [TestMethod]
        public void TestDateTimeWithFormat()
        {
            PropertyReader reader = _readerFactory.Create("{DateTime:YYYY}");
            Assert.IsInstanceOfType(reader, typeof(FormattedPropertyReader));
            Assert.AreEqual(((FormattedPropertyReader)reader).FormatString, "YYYY");
        }

        [TestMethod]
        public void TestComplexFormats()
        {
            PropertyReader reader = _readerFactory.Create("{DateTime:HH:mm:ss}");
            Assert.AreEqual(((FormattedPropertyReader)reader).FormatString, "HH:mm:ss");

            reader = _readerFactory.Create("{DateTime:}");
            Assert.IsInstanceOfType(reader, typeof(DateTimePropertyReader));

            reader = _readerFactory.Create("{DateTime: }");
            Assert.AreEqual(((FormattedPropertyReader)reader).FormatString, " ");
        }

        [TestMethod]
        public void TestProcessSubProperty()
        {
            PropertyReader reader = _readerFactory.Create("{Process.ProcessName}");
            Assert.IsInstanceOfType(reader, typeof(FastPropertyReader), "Expected FastPropertyReader instance");
            object value;
            Assert.IsTrue(reader.TryGetValue(out value, null, null, TraceEventType.Information, 0, null, null), "Reader didn't read");
            Assert.AreEqual<string>(Process.GetCurrentProcess().ProcessName, (string)value, "Wrong process name returned");
        }

        [TestMethod]
        public void TestProcessDeepSubProperty()
        {
            PropertyReader reader = _readerFactory.Create("{Process.MainModule.FileVersionInfo.FileName}");
            Assert.IsInstanceOfType(reader, typeof(FastPropertyReader), "Expected FastPropertyReader instance");
            object value;
            Assert.IsTrue(reader.TryGetValue(out value, null, null, TraceEventType.Information, 0, null, null), "Reader didn't read");
            Assert.AreEqual<string>(Process.GetCurrentProcess().MainModule.FileVersionInfo.FileName, (string)value, "Wrong filename returned");
        }


        [TestMethod]
        public void TestBuiltInTokens()
        {
            PropertyReaderFactory factory = new PropertyReaderFactory(false, true);
            
            Assert.IsInstanceOfType(factory.Create("{Message}"), typeof(MessagePropertyReader));
            Assert.IsInstanceOfType(factory.Create("{Id}"), typeof(IdPropertyReader));
            Assert.IsInstanceOfType(factory.Create("{ThreadId}"), typeof(ThreadIdPropertyReader));
            Assert.IsInstanceOfType(factory.Create("{ProcessId}"), typeof(ProcessIdPropertyReader));
            Assert.IsInstanceOfType(factory.Create("{ProcessName}"), typeof(ProcessNamePropertyReader));
            Assert.IsInstanceOfType(factory.Create("{Process}"), typeof(ProcessPropertyReader));
            Assert.IsInstanceOfType(factory.Create("{Callstack}"), typeof(CallstackPropertyReader));
            Assert.IsInstanceOfType(factory.Create("{DateTime}"), typeof(DateTimePropertyReader));
            Assert.IsInstanceOfType(factory.Create("{EventType}"), typeof(EventTypePropertyReader));
            Assert.IsInstanceOfType(factory.Create("{Source}"), typeof(SourcePropertyReader));
            Assert.IsInstanceOfType(factory.Create("{ActivityId}"), typeof(ActivityIdPropertyReader));
            Assert.IsInstanceOfType(factory.Create("{RelatedActivityId}"), typeof(RelatedActivityIdPropertyReader));
            Assert.IsInstanceOfType(factory.Create("{MachineName}"), typeof(MachineNamePropertyReader));
            Assert.IsInstanceOfType(factory.Create("{Timestamp}"), typeof(TimestampPropertyReader));
            Assert.IsInstanceOfType(factory.Create("{PrincipalName}"), typeof(PrincipalNamePropertyReader));
            Assert.IsInstanceOfType(factory.Create("{WindowsIdentity}"), typeof(WindowsIdentityPropertyReader));
            Assert.IsInstanceOfType(factory.Create("{LocalTime}"), typeof(LocalTimePropertyReader));

            Assert.IsInstanceOfType(factory.Create("{Year}"), typeof(DatePartPropertyReader));
            Assert.IsInstanceOfType(factory.Create("{Month}"), typeof(DatePartPropertyReader));
            Assert.IsInstanceOfType(factory.Create("{Day}"), typeof(DatePartPropertyReader));
            Assert.IsInstanceOfType(factory.Create("{Hour}"), typeof(DatePartPropertyReader));
            Assert.IsInstanceOfType(factory.Create("{Minute}"), typeof(DatePartPropertyReader));
            Assert.IsInstanceOfType(factory.Create("{Second}"), typeof(DatePartPropertyReader));
            Assert.IsInstanceOfType(factory.Create("{Millisecond}"), typeof(DatePartPropertyReader));

        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestNullToken()
        {
            _readerFactory.AddToken(null, new CustomPropertyReader());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestNullPropertyReader()
        {
            _readerFactory.AddToken("SomeToken", null);
        }

        [TestMethod]
        public void TestCustomReader()
        {
            _readerFactory.AddToken("AnotherCustomToken", new CustomPropertyReader());
            PropertyReader Reader = _readerFactory.Create("{AnotherCustomToken}");
            Assert.AreEqual(Reader.GetType(), typeof(CustomPropertyReader));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestTokenAlreadyExists()
        {
            _readerFactory.AddToken("Message", new MessagePropertyReader());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void InvalidToken()
        {
            _readerFactory.Create(
                "{UtterGarbage}");
        }

        [TestMethod]
        public void TestCustomPropertyReader_WithConfig()
        {
            TraceSource source = new TraceSource("source8");
            Assert.AreEqual(0, InMemoryTraceListener.TraceObjects.Count);
            source.TraceEvent(TraceEventType.Information, 0);
            Assert.AreEqual(0, InMemoryTraceListener.TraceObjects.Count);
            source.TraceEvent(TraceEventType.Error, 0);
            Assert.AreEqual(1, InMemoryTraceListener.TraceObjects.Count);
            source.TraceEvent(TraceEventType.Critical, 0);
            Assert.AreEqual(2, InMemoryTraceListener.TraceObjects.Count);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestTokenAlreadyExistsErrors()
        {
            IPropertyReaderFactory factory = new PropertyReaderFactory(false, false);

            factory.AddToken("Id", new IdPropertyReader());
            factory.AddToken("Id", new IdPropertyReader());
        }

        [TestMethod]
        [ExpectedException(typeof(ConfigurationErrorsException))]
        public void TestReadInvalidTokenConfigurationError()
        {
            // Get the configuration file location in a way that works with MSTest and TestDriven.NET's host
            string originalFile = AppDomain.CurrentDomain.SetupInformation.ConfigurationFile;
            string dir = Path.GetDirectoryName(originalFile);
            string tempFile = Path.Combine(dir, "temp.config");

            // And attempt to rename the file
            if (File.Exists(originalFile))
            {
                // Now try to get the config section...
                try
                {
                    // Rename the config file so it's unavailable to the test
                    File.Move(originalFile, tempFile);
                    // Create the new file from the resource
                    Stream src = Assembly.GetExecutingAssembly().GetManifestResourceStream("Ukadc.Diagnostics.Tests.Config2.config");
                    using (FileStream fs = new FileStream(originalFile, FileMode.CreateNew, FileAccess.Write))
                    {
                        StreamHelper.CloneStream(src, fs);
                    }

                    // Now refresh configuration
                    ConfigurationManager.RefreshSection("ukadc.diagnostics");

                    try
                    {
                        // This should throw an exception
                        new PropertyReaderFactory(true, false);
                    }
                    catch (ConfigurationErrorsException exc)
                    {
                        // and we need to check the content of the error message to be sure we are exercising the correct exception
                        Assert.AreEqual("You can only specify one of format, type and dynamicProperty on a token element (token name='{CustomToken}').", exc.Message); 
                        throw;
                    }
                    // if we get here, something has gone wrong
                    Assert.Fail("if we get here, something has gone wrong");
                }
                finally
                {
                    if (File.Exists(originalFile))
                    {
                        // delete the original file
                        File.Delete(originalFile);
                    }

                    // Rename it back
                    File.Move(tempFile, originalFile);

                    // Ensure that the next test will read the right stuff...
                    ConfigurationManager.RefreshSection("ukadc.diagnostics");
                }
            }
        }
    }

    public class CustomPropertyReader : PropertyReader
    {
        private NumericComparator _numericComparator = new NumericComparator();

        public override Type PropertyType
        {
            get { return typeof(TraceEventType); }
        }

        public override IComparator Comparator
        {
            get { return _numericComparator; }
        }

        public override bool TryGetValue(out object value, TraceEventCache cache, string source, TraceEventType eventType, int id, string formatOrMessage, object[] data)
        {
            value = eventType;
            return true;
        }
    }
}
