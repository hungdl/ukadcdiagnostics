using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ukadc.Diagnostics.Listeners;
using System.ServiceModel;

namespace Ukadc.Diagnostics.ProxyTraceViewer
{
    [ServiceBehavior(InstanceContextMode=InstanceContextMode.Single)]
    public class ProxyTraceService : IProxyTraceService
    {
        private readonly Action<TraceEvent> _eventReceived;

        public ProxyTraceService(Action<TraceEvent> eventReceived)
        {
            if (eventReceived == null)
            {
                throw new ArgumentNullException("eventReceived");
            }

            _eventReceived = eventReceived;
        }

        public void SendTraceEvent(TraceEvent traceEvent)
        {
            _eventReceived(traceEvent);
        }
    }
}
