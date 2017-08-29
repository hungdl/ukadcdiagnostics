using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ukadc.Diagnostics.Listeners;
using System.Diagnostics;
using System.ServiceModel;
using System.IO;
using Ukadc.Diagnostics.Utils;
using System.Threading;

namespace Ukadc.Diagnostics.Tests.Listeners
{
    [TestClass]
    public class TestProxyTraceListener
    {
        [TestMethod]
        public void TestBasicMessage()
        {
            using (MiniProxyService service = new MiniProxyService())
            {
                ManualResetEvent mre = new ManualResetEvent(false);
                service.EventReceived += delegate
                {
                    mre.Set();
                };
                ProxyTraceListener listener = new ProxyTraceListener();
                TraceSource source = new TraceSource("TestTraceSource", SourceLevels.All);
                source.Listeners.Add(listener);

                source.TraceEvent(TraceEventType.Critical, 0, "boo!");

                Assert.IsTrue(mre.WaitOne(500));
                Assert.AreEqual(1, service.Events.Count);
            }
        }

        [TestMethod]
        public void TestMessageAfterFailedMessage()
        {
            MockInternalLogger mil = new MockInternalLogger();

            TraceSource source = new TraceSource("TestTraceSource", SourceLevels.All);

            // only need to override internal logger during listener creation
            using (DefaultServiceLocator.Instance.OverrideType<IInternalLogger>(mil))
            {
                ProxyTraceListener listener = new ProxyTraceListener();
                source.Listeners.Add(listener);
            }

            Assert.AreEqual(0, mil.Exceptions.Count);

            // no listener, this will fail (albeit quietly).
            source.TraceEvent(TraceEventType.Critical, 0, "boo!");

            // check internal logger saw the exception
            Assert.AreEqual(1, mil.Exceptions.Count);

            using (MiniProxyService service = new MiniProxyService())
            {
                ManualResetEvent mre = new ManualResetEvent(false);
                service.EventReceived += delegate
                {
                    mre.Set();
                };
                source.TraceEvent(TraceEventType.Critical, 0, "boo!");

                Assert.IsTrue(mre.WaitOne(500));
                Assert.AreEqual(1, service.Events.Count);
            }
            
            Assert.AreEqual(1, mil.Exceptions.Count);
        }


        [TestMethod]
        public void TestWithConfig()
        {
            using (MiniProxyService service = new MiniProxyService(
                NetNamedPipeSecurityMode.None, 
                new Uri("net.pipe://localhost/ProxyTraceService2/")))
            {
                ManualResetEvent mre = new ManualResetEvent(false);
                service.EventReceived += delegate
                {
                    mre.Set();
                };

                TraceSource source = new TraceSource("source15");
                source.TraceInformation("boo!");

                Assert.IsTrue(mre.WaitOne(500));
                Assert.AreEqual(1, service.Events.Count);
            }
        }
    }

    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class MiniProxyService : IDisposable, IProxyTraceService
    {
        private List<TraceEvent> _events = new List<TraceEvent>();
        private ServiceHost _host;

        public MiniProxyService()
            : this(NetNamedPipeSecurityMode.None)
        { }

        public MiniProxyService(NetNamedPipeSecurityMode mode)
            : this(mode,  ProxyTraceListener.DefaultEndpointAddress)
        {}

        public MiniProxyService(NetNamedPipeSecurityMode mode, Uri endpointUri)
        {
            _host = new ServiceHost(
                this,
                endpointUri);

            _host.AddServiceEndpoint(
                typeof(IProxyTraceService),
                new NetNamedPipeBinding(mode),
                endpointUri);

            _host.Open();
        }

        public List<TraceEvent> Events
        {
            get
            {
                return _events;
            }
        }

        public event EventHandler EventReceived;

        public void SendTraceEvent(TraceEvent traceEvent)
        {
            _events.Add(traceEvent);
            if (EventReceived != null)
            {
                EventReceived(this, EventArgs.Empty);
            }
        }

        public void Dispose()
        {
            _host.Abort();
            //_host.Close(TimeSpan.FromMilliseconds(100));
        }
    }

}
