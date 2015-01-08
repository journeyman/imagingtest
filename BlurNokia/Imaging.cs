using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using Lumia.Imaging;
using Lumia.Imaging.Adjustments;

namespace BlurNokia
{
    public class Imaging
    {
        public async Task<WriteableBitmap> Blur(Stream stream, int kernelSize)
        {
            using (var source = new StreamImageSource(stream))
            {
                using (var filters = new FilterEffect(source))
                {
                    var blur = new BlurFilter(kernelSize);
                    filters.Filters = new[] {blur};

                    //var output = new WriteableBitmap(pixelWidth, pixelHeight);
                    using (var renderer = new WriteableBitmapRenderer(filters))
                    {
                        return await renderer.RenderAsync();
                    }
                }
            }

        }
    }
}
