using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using ImagingTest.Resources;

namespace ImagingTest
{
    public partial class MainPage : PhoneApplicationPage
    {
        public MainPage()
        {
            InitializeComponent();

            Loaded += async (sender, args) =>
            {
                var dur = Perf.Checkpoint("Operation1");
                Log.Items.Add(dur + " - started");
                await Task.Delay(2000);
                dur = Perf.Finish(dur.Title);
                Log.Items.Add(dur + " - finished");

                dur = Perf.Checkpoint("Operation2");
                Log.Items.Add(dur + " - started");
                await Task.Delay(3000);
                dur = Perf.Checkpoint("Operation2");
                Log.Items.Add(dur + " - checkpoint");
                await Task.Delay(3000);
                dur = Perf.Finish(dur.Title);
                Log.Items.Add(dur + " - finished");
            };
        }

    }
}