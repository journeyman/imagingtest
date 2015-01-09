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
    using Windows.Networking.Connectivity;

    using Microsoft.Phone.Net.NetworkInformation;

    public class TestData
    {
        public long MemoryUsed { get; set; }
        public TimeSpan TimeUsed { get; set; }
    }

    public partial class MainPage : PhoneApplicationPage
    {
        public MainPage()
        {
            InitializeComponent();

            Loaded += async (sender, args) =>
                {
                    var hosts = NetworkInformation.GetHostNames().ToList();
                    var profile = NetworkInformation.GetInternetConnectionProfile();
                    var client = new WebClient();
                    client.DownloadStringCompleted += (s, e) =>
                    {
                        var error = e.Error;
                    };
                    client.DownloadStringAsync(new Uri("https://journeyman.fwd.wf?log=" + Uri.EscapeUriString("test message from wp")));
                    //await DoTheTest();
                    //await DoTheTest();

                    //var data = new List<TestData>();
                    //foreach (var _ in Enumerable.Repeat(0, 10))
                    //{
                    //    data.Add(await this.DoTheTest());
                    //}

                    //DumpTestData(data);
                };
        }


        private void DumpTestData(IEnumerable<TestData> testData)
        {
            LogMessage("\n\n");
            long totalMsec = 0;
            long totalMem = 0;
            int i = 0;
            foreach (var data in testData)
            {
                i++;
                totalMsec += (long)data.TimeUsed.TotalMilliseconds;
                totalMem += data.MemoryUsed;
                var str = string.Format("Time: {0}, Memory: {1}", data.TimeUsed.ToMinSecsMsecs(), data.MemoryUsed);
                LogMessage(str);
            }
            var timeAvg = TimeSpan.FromMilliseconds(totalMsec / i);
            var memAvg = totalMem / i;

            LogMessage("--------------------------");
            var finalMsg = string.Format("Avg Time: {0}, Avg Mem: {1}", timeAvg.ToMinSecsMsecs(), memAvg.ToPrettyMbString());
            LogMessage(finalMsg);
            var client = new WebClient();
            client.DownloadStringCompleted += (s, e) =>
                {
                    var error = e.Error;
                };
            client.DownloadStringAsync(new Uri("http://192.168.26.150:7777/?log=" + Uri.EscapeUriString(finalMsg)));
        }

        private async Task<TestData> DoTheTest()
        {
            var stream = await GetImage();
            var memBefore = Memory.Snapshot();
            Perf.Checkpoint("Nokia Blur");
            var image = await BlurNokia.Imaging.Blur(stream, 17);
            var time = Perf.Finish("Nokia Blur");
            var memory = Memory.Snapshot() - memBefore;
            LogMessage("Time: " + time);
            LogMessage(string.Format("Memory: {0}", memory.ToPrettyMbString()));
            Image.Source = image;
            GC.Collect();
            GC.WaitForPendingFinalizers();
            return new TestData { MemoryUsed = memory, TimeUsed = time.Elapsed };
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