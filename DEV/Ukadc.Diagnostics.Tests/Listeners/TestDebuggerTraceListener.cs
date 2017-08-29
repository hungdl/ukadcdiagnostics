using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ukadc.Diagnostics.Listeners;
using System.Diagnostics;

namespace Ukadc.Diagnostics.Tests.Listeners
{
    [TestClass]
    public class TestDebuggerTraceListener
    {
        [TestMethod]
        public void TestDebugger()
        {
            DebuggerTraceListener debugger = new DebuggerTraceListener(@"{EventType}: {Id}, {Source}, {Message} ({RelatedActivityId})");
            debugger.TraceEvent(null, "MySource", System.Diagnostics.TraceEventType.Critical, 32, "Hello from the DebuggerTraceListener");
            debugger.TraceData(null, "MySource", TraceEventType.Information, 23, "Data!");
            debugger.TraceTransfer(null, "MySource", 21, "Transferring...", Guid.NewGuid());

            // IMPORTANT
            // TODO Need to look at whether there is a viable way of testing the debugger output
        }
    }
}
