using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Threading;
using System.Diagnostics;
using Ukadc.Diagnostics.Listeners;
using System.Windows.Threading;

namespace Ukadc.Diagnostics.ProxyTraceViewer
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Main : Window
    {
        public Main()
        {
            InitializeComponent();

            this.DataContext = new MainViewModel();

            StartDummyFeed();
        }

        [Obsolete("This is a horrible hack for now..")]
        private void StartDummyFeed()
        {
            TraceSource source = new TraceSource("TestingDummySource", SourceLevels.All);
            source.Listeners.Add(new ProxyTraceListener());

            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(1000);
            timer.Tick += delegate
                {
                    var types = Enum.GetValues(typeof(TraceEventType));
                    Random random = new Random();
                    int index = random.Next(0, types.Length - 1);

                    source.TraceEvent((TraceEventType)types.GetValue(index), 0, "Morgan is a cider drinker");
                };
            timer.Start();
        }
    }
}
