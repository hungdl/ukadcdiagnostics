using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Windows;

namespace Ukadc.Diagnostics.ProxyTraceViewer.Contracts
{
    /// <summary>
    /// Class representing the DataTemplate to be used to display <see cref="TraceEvent" />s.
    /// </summary>
    public class DisplayTemplate : INotifyPropertyChanged
    {
        private string _name;

        /// <summary>
        /// The name of the DisplayTemplate 
        /// </summary>
        /// <remarks>This is typically used on a Tab or Tooltip in the user interface</remarks>
        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                OnPropertyChanged("Name");
            }
        }

        private DataTemplate _dataTemplate;

        /// <summary>
        /// The DataTemplate used to display a <see cref="TraceEvent" />
        /// </summary>
        public DataTemplate DataTemplate
        {
            get { return _dataTemplate; }
            set
            {
                _dataTemplate = value;
                OnPropertyChanged("DataTemplate");
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
}
