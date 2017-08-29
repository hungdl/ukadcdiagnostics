using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ukadc.Diagnostics.Listeners;
using Ukadc.Diagnostics.Utils;
using System.IO;
using System.Threading;
using System.Diagnostics;
using System.Configuration;

namespace Ukadc.Diagnostics.Tests.Listeners
{
    [TestClass]
    public class TestFileTraceListener
    {
        [TestMethod]
        public void TestFile()
        {
            using (DefaultServiceLocator.Instance.OverrideType<IStreamWriterCache, MockStreamWriterCache>())
            {
                Trace.AutoFlush = true;

                FileTraceListener listener = new FileTraceListener();
                listener.Attributes["filePath"] = "testFilePath";
                listener.Attributes["output"] = "output";
                listener.Attributes["cleanInterval"] = "01:00:00";

                TraceSource source = new TraceSource("Test", SourceLevels.All);
                source.Listeners.Add(listener);
                source.TraceEvent(TraceEventType.Critical, 0, "test message");

                MockStreamWriterCache cache = (MockStreamWriterCache) 
                    DefaultServiceLocator.Instance.GetService<IStreamWriterCache>();

                Stream s = cache.GetStreamWriter("testFilePath").BaseStream;
                s.Seek(0, SeekOrigin.Begin);
                using (StreamReader sr = new StreamReader(s))
                {
                    string output = sr.ReadToEnd();
                    Assert.AreEqual("output" + Environment.NewLine, output);
                }

                Assert.AreEqual(0, cache.ClearCounter);
            }
        }

        [TestMethod]
        public void TestZeroInterval()
        {
            FileTraceListener listener;
            MockInternalLogger mil = new MockInternalLogger();
            using (DefaultServiceLocator.Instance.OverrideType<IInternalLogger>(mil))
            {
                listener = new FileTraceListener();
                listener.Attributes["filePath"] = "testFilePath";
                listener.Attributes["output"] = "output";
                listener.Attributes["cleanInterval"] = "00:00:00";

                listener.TraceData(null, null, TraceEventType.Error, 0, null);
            }
            Assert.AreEqual(0, mil.Exceptions.Count);
            Assert.AreEqual(1, mil.Messages.Count);

            Assert.AreEqual(FileTraceListener.DEFAULT_CLEAN_INTERVAL, listener.CleanInterval);
        }

        [TestMethod]
        public void TestNoInterval()
        {
            FileTraceListener listener;
            MockInternalLogger mil = new MockInternalLogger();
            using (DefaultServiceLocator.Instance.OverrideType<IInternalLogger>(mil))
            {
                listener = new FileTraceListener();
                listener.Attributes["filePath"] = "testFilePath";
                listener.Attributes["output"] = "output";
                listener.Attributes["cleanInterval"] = string.Empty;

                listener.TraceData(null, null, TraceEventType.Error, 0, null);
            }
            Assert.AreEqual(0, mil.Exceptions.Count);
            Assert.AreEqual(1, mil.Messages.Count);

            Assert.AreEqual(FileTraceListener.DEFAULT_CLEAN_INTERVAL, listener.CleanInterval);
        }

        [TestMethod]
        public void TestInterval()
        {
            DateTime start; 

            using (DefaultServiceLocator.Instance.OverrideType<IStreamWriterCache, MockStreamWriterCache>())
            {
                Trace.AutoFlush = true;

                FileTraceListener listener = new FileTraceListener();
                listener.Attributes["filePath"] = "testFilePath";
                listener.Attributes["output"] = "output";
                listener.Attributes["cleanInterval"] = "00:00:00.5";

                TraceSource source = new TraceSource("Test", SourceLevels.All);
                source.Listeners.Add(listener);
                source.TraceEvent(TraceEventType.Critical, 0, "test message");
                // should have initialized here and the timer have started
                start = DateTime.Now; 

                MockStreamWriterCache cache = (MockStreamWriterCache)
                    DefaultServiceLocator.Instance.GetService<IStreamWriterCache>();

                Thread.Sleep(1100);

                Assert.AreEqual(2, cache.ClearCounter);
            }
        }

        [TestMethod]
        public void TestMockStreamWriterCache()
        {
            MockStreamWriterCache cache = new MockStreamWriterCache();

            DateTime start = DateTime.Now;
            StreamWriter a1 = cache.GetStreamWriter("a");
            StreamWriter b1 = cache.GetStreamWriter("b");
            StreamWriter a2 = cache.GetStreamWriter("a");
            StreamWriter b2 = cache.GetStreamWriter("b");

            Assert.AreNotEqual(a1, b1);
            Assert.AreEqual(a1, a2);
            Assert.AreEqual(b1, b2);

            cache.ClearOldStreams(start);

            // should still be the same
            Assert.AreEqual(a1, cache.GetStreamWriter("a"));

            cache.ClearOldStreams(DateTime.Now.AddSeconds(1));

            // should be a new StreamWriter now
            Assert.AreNotEqual(a1, cache.GetStreamWriter("a"));
        }

        [TestMethod]
        public void TestFileWithConfiguration()
        {
            MockStreamWriterCache mock = new MockStreamWriterCache();

            string message = Guid.NewGuid().ToString();
            TraceSource source = new TraceSource("source14");

            using (DefaultServiceLocator.Instance.OverrideType<IStreamWriterCache>(mock))
            {
                // kicks configuration into life
                source.TraceEvent(TraceEventType.Verbose, 0, message);
            }

            FileTraceListener listener = source.Listeners[1] as FileTraceListener;

            Assert.IsNotNull(listener, "listener was expected to be of type FileTraceListener");

            Assert.AreEqual(listener.FilePath, "log.txt");
            Assert.AreEqual(listener.Output, "{ActivityId}");
            Assert.AreEqual(listener.CleanInterval, TimeSpan.FromSeconds(1));

        }

        private class MockStreamWriterCache : StreamWriterCache
        {
            public int ClearCounter;

            protected override Stream GetStream(string path)
            {
                return new MemoryStream();
            }

            public override void ClearOldStreams(DateTime notUsedSince)
            {
                Interlocked.Increment(ref ClearCounter);
                base.ClearOldStreams(notUsedSince);
            }
        }
    }
}
