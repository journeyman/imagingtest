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
            LogMessage(string.Format("Before image loading:\n{0}", Memory.Snapshot()));
            var uri = new Uri("ms-appx:///Resources/images/photo2_big.jpg");
            var file = await StorageFile.GetFileFromApplicationUriAsync(uri);
            var stream = await file.OpenStreamForReadAsync();
            LogMessage(Perf.Checkpoint("Nokia Blur") + ": before");
            var image = await BlurNokia.Imaging.Blur(stream, 17);
            LogMessage(Perf.Checkpoint("Nokia Blur") + ": after");
            LogMessage(string.Format("After image loading:\n{0}", Memory.Snapshot()));
            Image.Source = image;
        }

        private void LogMessage(string msg)
        {
            Log.Items.Add(msg);
            Debug.WriteLine(msg);
        }
    }
}