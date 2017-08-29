using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Ukadc.Diagnostics.ProxyTraceViewer.Contracts
{
    public interface IDisplayTraceEvents
    {
        /// <summary>
        /// Tests whether this IDisplayTraceEvents instance is applicable to the specified TraceEvent
        /// </summary>
        /// <param name="traceEvent">The traceEvent to be checked for applicability</param>
        /// <returns>true if traceEvent is applicable</returns>
        bool IsApplicable(TraceEvent traceEvent);

        /// <summary>
        /// Gets the DataTemplate to be used in ListView
        /// </summary>
        /// <returns>The appropriate DataTemplate or null if this IDisplayTraceEvents instance does not support List templating</returns>
        DataTemplate GetListItemTemplate();
        
        /// <summary>
        /// Gets the DisplayTemplate, which includes the name and DataTemplate that should be used to display the 
        /// TraceEvent
        /// </summary>
        /// <returns>DisplayTemplate to Display TraceEvents</returns>
        DisplayTemplate GetDisplayTemplate();

    }
}
