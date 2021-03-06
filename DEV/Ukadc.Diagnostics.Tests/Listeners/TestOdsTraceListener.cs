﻿using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ukadc.Diagnostics.Listeners;
using Ukadc.Diagnostics.Configuration;
using System.Diagnostics;

namespace Ukadc.Diagnostics.Tests.Listeners
{
    [TestClass]
    public class TestOdsTraceListener
    {
        [TestMethod]
        public void TestOds()
        {
            OutputDebugStringTraceListener ods = new OutputDebugStringTraceListener(@"{EventType}: {Id}, {Source}, {Message} ({RelatedActivityId})");
            ods.TraceEvent(null, "MySource", System.Diagnostics.TraceEventType.Critical, 32, "Hello from the ODSTraceListener");
            ods.TraceData(null, "MySource", TraceEventType.Information, 23, "Data!");
            ods.TraceTransfer(null, "MySource", 21, "Transferring...", Guid.NewGuid());

            // IMPORTANT
            // Practically impossible to make any sensible assertions here unless we hook into OutputDebugString ourselves. Happier being
            // sure the code is 'covered' though. 
            // TODO - consider making the SafeNativeMethods class 'pluggable' so we can mock it out. That won't actually increase
            // confidence much though because most of the code is inside CustomTraceListener for which we have good coverage and assertions
        }
    }
}
