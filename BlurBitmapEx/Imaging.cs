using System.IO;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace BlurBitmapEx
{
    public static class Imaging
    {
        public static async Task<WriteableBitmap> Blur(Stream stream, int kernelSize)
        {
            var image = new BitmapImage();
            using (stream)
            {
                image.SetSource(stream);
                var bitmap = new WriteableBitmap(image);
                return bitmap.Convolute(WriteableBitmapExtensions.KernelGaussianBlur5x5);
            }
        }
    }
}
