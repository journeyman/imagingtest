using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Windows.Storage;
using BlurBitmapEx;
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
                DoTheTest();
            };
        }

        private async void DoTheTest()
        {
            var stream = await GetImage();
            var memBefore = Memory.Snapshot();
            Perf.Checkpoint("Nokia Blur");
            var image = BlurBitmapEx.Imaging.Blur(stream, 17);
            LogMessage("Time: " + Perf.Checkpoint("Nokia Blur"));
            LogMessage(string.Format("Memory: {0}", (Memory.Snapshot() - memBefore).ToPrettyMbString()));
            Image.Source = image;
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        public async Task<Stream> GetImage()
        {
            TaskCompletionSource<Stream> tcs = new TaskCompletionSource<Stream>();
            var uri = new Uri("http://lorempixel.com/200/200/people/");
            var client = new WebClient();
            client.OpenReadCompleted += (sender, args) =>
                {
                    if (args.Error == null)
                    {
                        tcs.SetResult(args.Result);
                    }
                    else
                    {
                        tcs.SetException(args.Error);
                    }
                };
            client.OpenReadAsync(uri);
            return await tcs.Task;
        }

        private void LogMessage(string msg)
        {
            Log.Items.Add(msg);
            Debug.WriteLine(msg);
        }
    }
}