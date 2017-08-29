using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ukadc.Diagnostics.ProxyTraceViewer.Contracts;
using System.Windows;
using System.Windows.Markup;
using System.Reflection;

namespace Ukadc.Diagnostics.ProxyTraceViewer
{
    public class DefaultDisplayTraceEvents : IDisplayTraceEvents
    {
        private readonly DataTemplate _listTemplate;
        private readonly DataTemplate _viewTemplate;

        public DefaultDisplayTraceEvents()
        {
            ResourceDictionary dictionary = (ResourceDictionary) XamlReader.Load(Assembly.GetExecutingAssembly()
                .GetManifestResourceStream("Ukadc.Diagnostics.ProxyTraceViewer.DefaultDisplayTraceEventrs.DefaultDisplayTraceEventsDictionary.xaml"));

            _listTemplate = (DataTemplate)dictionary["listTemplate"];
            _viewTemplate = (DataTemplate)dictionary["viewTemplate"];

        }

        /// <inheritdoc />
        public bool IsApplicable(TraceEvent traceEvent)
        {
            return true;
        }

        /// <inheritdoc />
        public DataTemplate GetListItemTemplate()
        {
            return _listTemplate;
        }

        /// <inheritdoc />
        public DisplayTemplate GetDisplayTemplate()
        {
            return new DisplayTemplate { Name = "Raw Text View", DataTemplate = _viewTemplate };
        }
    }
}
