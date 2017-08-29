using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ServiceModel;
using Ukadc.Diagnostics.Listeners;
using Ukadc.Diagnostics.ProxyTraceViewer.Contracts;
using System.Windows;

namespace Ukadc.Diagnostics.ProxyTraceViewer
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private readonly ServiceHost _serviceHost;
        private readonly List<IDisplayTraceEvents> _viewers = new List<IDisplayTraceEvents>();

        public MainViewModel()
        {
            ProxyTraceService service = new ProxyTraceService(EventReceived);

            _serviceHost = new ServiceHost(
                service,
                ProxyTraceListener.DefaultEndpointAddress);

            _serviceHost.AddServiceEndpoint(
                typeof(IProxyTraceService),
                new NetNamedPipeBinding(NetNamedPipeSecurityMode.None),
                ProxyTraceListener.DefaultEndpointAddress);

            _serviceHost.Open();

            _viewers.Add(new DefaultDisplayTraceEvents());
        }

        private void EventReceived(TraceEvent traceEvent)
        {
            DisplayTraceEvent dte = new DisplayTraceEvent
            {
                TraceEvent = traceEvent,
            };

            foreach (IDisplayTraceEvents idt in _viewers)
            {
                if (idt.IsApplicable(traceEvent))
                {
                    dte.DisplayTemplates.Add(idt.GetDisplayTemplate());

                    DataTemplate listTemplate = idt.GetListItemTemplate();

                    if (listTemplate != null)
                    {
                        dte.ListTemplate = listTemplate;
                    }
                }
            }

            DisplayTraceEvents.Add(dte);
            SelectedTab = 0;
        }

        private readonly ObservableCollection<DisplayTraceEvent> _displayTraceEvents = new ObservableCollection<DisplayTraceEvent>();

        public ObservableCollection<DisplayTraceEvent> DisplayTraceEvents
        {
            get { return _displayTraceEvents; }
        }

        private DisplayTraceEvent _selectedEvent;

        public DisplayTraceEvent SelectedEvent
        {
            get { return _selectedEvent; }
            set
            {
                _selectedEvent = value;
                OnPropertyChanged("SelectedEvent");
            }
        }

        private int _selectedTab;

        public int SelectedTab
        {
            get { return _selectedTab; }
            set
            {
                _selectedTab = value;
                OnPropertyChanged("SelectedTab");
            }
        }
      
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler pceh = PropertyChanged;
            if (pceh != null)
            {
                pceh(this, new PropertyChangedEventArgs(propertyName));
            }
        }

    }

    public class DisplayTraceEvent : INotifyPropertyChanged
    {
        private TraceEvent _traceEvent;

        public TraceEvent TraceEvent
        {
            get { return _traceEvent; }
            set
            {
                _traceEvent = value;
                OnPropertyChanged("TraceEvent");
            }
        }


        private DataTemplate _listTemplate;

        public DataTemplate ListTemplate
        {
            get { return _listTemplate; }
            set
            {
                _listTemplate = value;
                OnPropertyChanged("ListTemplate");
            }
        }

        private readonly ObservableCollection<DisplayTemplate> _displayTemplates = new ObservableCollection<DisplayTemplate>();

        public ObservableCollection<DisplayTemplate> DisplayTemplates
        {
            get { return _displayTemplates; }
        }
        

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler pceh = PropertyChanged;
            if (pceh != null)
            {
                pceh(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
