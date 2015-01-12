using System.IO;
using System.Windows.Media.Imaging;

namespace BlurBitmapEx
{
    using Windows.Foundation;

    using WriteableBitmapExtensions;

    public static class Imaging
    {
        public static WriteableBitmap Blur(Stream stream, int kernelSize, Size size)
        {
            var image = new BitmapImage();
            using (stream)
            {
                image.SetSource(stream);
            }
            var bitmap = new WriteableBitmap(image);
            bitmap = bitmap.Resize((int)size.Width, (int)size.Height, Interpolation.Bilinear);
            return bitmap.Convolute(WriteableBitmapExtensions.KernelGaussianBlur5x5);
        }
    }
}
