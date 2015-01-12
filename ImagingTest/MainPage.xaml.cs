namespace ImagingTest
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.IO.IsolatedStorage;
    using System.Net;
    using System.Threading.Tasks;
    using System.Windows;
    using Microsoft.Phone.Controls;

    using Size = Windows.Foundation.Size;

    public partial class MainPage : PhoneApplicationPage
    {
        public MainPage()
        {
            InitializeComponent();

            Loaded += async (sender, args) =>
                {
                    var data = await DoTheTest();
                    DumpSingleTest(data);
                    
                    Application.Current.Terminate();
                };
        }

        private void DumpSingleTest(TestData testData)
        {
            var fileStorage = IsolatedStorageFile.GetUserStoreForApplication();
            var writer = new StreamWriter(new IsolatedStorageFileStream("dump.txt", FileMode.Append, fileStorage));
            
            writer.WriteLine("{0},{1}", testData.TimeUsed, testData.MemoryUsed);
            writer.Close();
            writer.Dispose();
        }

        private async Task<TestData> DoTheTest()
        {
            var stream = await GetImage();
            var memBefore = Memory.Snapshot();
            Perf.Checkpoint("Nokia Blur");
            var image = BlurBitmapEx.Imaging.Blur(stream, 17, new Size(15,15));
            var time = Perf.Finish("Nokia Blur");
            var memory = Memory.Snapshot() - memBefore;
            LogMessage("Time: " + time);
            LogMessage(string.Format("Memory: {0}", memory.ToPrettyMbString()));
            Image.Source = image;
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
        }
    }
}