// THIS CODE AND INFORMATION ARE PROVIDED AS IS WITHOUT WARRANTY OF ANY
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using Ukadc.Diagnostics.Utils.PropertyReaders;
using Ukadc.Diagnostics.Utils;

namespace Ukadc.Diagnostics.Listeners
{
    /// <summary>
    /// DebuggerTraceListener writes trace messages to the debugger using System.Diagnostics.Debugger.Log
    /// </summary>
    public class DebuggerTraceListener : CustomTraceListener
    {
        private PropertyReader _propertyReader;

        /// <summary>
        /// Creates a new instance of the DebuggerTraceListener class
        /// </summary>
        /// <param name="valueToken"></param>
        public DebuggerTraceListener(string valueToken)
            : base("DebuggerTraceListener")
        {
            _propertyReader = DefaultServiceLocator.GetService<IPropertyReaderFactory>().CreateCombinedReader(valueToken);
        }

        /// <summary>
        /// This method implements the logging for TraceEvent calls
        /// </summary>
        protected override void TraceEventCore(TraceEventCache eventCache, string source, TraceEventType eventType, int id, string message)
        {
            if (!ShouldLog()) // don't build the message if there's no-one listening
            {
                return;
            }
            object value;
            if (_propertyReader.TryGetValue(out value, eventCache, source, eventType, id, message, null))
            {
                Log(StringFormatter.SafeToString(value));
            }
        }

        /// <summary>
        /// This method implements the logging for TraceData calls
        /// </summary>
        protected override void TraceDataCore(TraceEventCache eventCache, string source, TraceEventType eventType, int id, params object[] data)
        {
            if (!ShouldLog()) // don't build the message if there's no-one listening
            {
                return;
            }
            object value;
            if (_propertyReader.TryGetValue(out value, eventCache, source, eventType, id, null, data))
            {
                Log(StringFormatter.SafeToString(value));
            }
        }

        private bool ShouldLog()
        {
            return Debugger.IsLogging();
        }

        private void Log(string message)
        {
            Debugger.Log(0, string.Empty, message);
        }
    }
}
