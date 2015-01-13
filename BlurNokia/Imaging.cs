using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using Lumia.Imaging;
using Lumia.Imaging.Adjustments;

namespace BlurNokia
{
    using Windows.Foundation;

    public static class Imaging
    {
        public static async Task<WriteableBitmap> Blur(Stream stream, int kernelSize, Size size)
        {
            using (var source = new StreamImageSource(stream))
            using (var filters = new FilterEffect(source))
            {
                var blur = new BlurFilter(kernelSize);

                filters.Filters = new[] { blur };

                using (var renderer = new BitmapRenderer(filters))
                {
                    renderer.Size = size;
                    renderer.OutputOption = OutputOption.PreserveAspectRatio;
                        
                    var bitmap = await renderer.RenderAsync();
                    var target = new WriteableBitmap((int)bitmap.Dimensions.Width, (int)bitmap.Dimensions.Height);

                    using (var bitmapSource = new BitmapImageSource(bitmap))
                    using (var rend = new WriteableBitmapRenderer(bitmapSource, target))
                    {
                        return await rend.RenderAsync().AsTask().ConfigureAwait(false);
                    }
                }
            }
        }

        public static async Task<WriteableBitmap> Blur2(Stream stream, int kernelSize, Size size)
        {
            using (var source = new StreamImageSource(stream))
            using (var renderer = new BitmapRenderer(source))
            {
                renderer.Size = size;
                renderer.OutputOption = OutputOption.PreserveAspectRatio;

                var bitmap = await renderer.RenderAsync();

                using (var bitmapSource = new BitmapImageSource(bitmap))
                using (var filters = new FilterEffect(bitmapSource))
                {
                    var blur = new BlurFilter(kernelSize);

                    filters.Filters = new[] { blur };

                    var target = new WriteableBitmap((int)bitmap.Dimensions.Width, (int)bitmap.Dimensions.Height);

                    using (var rend = new WriteableBitmapRenderer(filters, target))
                    {
                        return await rend.RenderAsync().AsTask().ConfigureAwait(false);
                    }
                }
            }
        }

    }
}
