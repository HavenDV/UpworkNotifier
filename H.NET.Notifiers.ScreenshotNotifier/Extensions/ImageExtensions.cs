using System.Drawing;

namespace H.NET.Notifiers.ScreenshotNotifier.Extensions
{
    public static class ImageExtensions
    {
        public static Bitmap ToBitmap(this Image image) => new Bitmap(image);
    }
}
