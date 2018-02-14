using System.Drawing;
using Emgu.CV;
using H.NET.Notifiers.Extensions;
using H.NET.Notifiers.Utilities;

// ReSharper disable once CheckNamespace
namespace H.NET.Notifiers
{
    public class UpworkScreenshotNotifier : ScreenshotTimerNotifier
    {
        #region Properties

        private string ExamplePath { get; }
        private Mat Mask { get; set; } 

        #endregion

        #region Constructors

        public UpworkScreenshotNotifier(string examplePath, int interval) : base(interval)
        {
            ExamplePath = examplePath;

            Mask = new Mat(ExamplePath).ToGray();
        }

        #endregion

        #region BaseScreenshotTimerNotifier

        protected override bool Analyze(Bitmap bitmap)
        {
            using (var mat = bitmap.ToMat())
            using (var grayMat = mat.ToGray())
            {
                return ScreenshotUtilities.IsEquals(grayMat, Mask, Mask);
            }
        }

        #endregion

        #region IDisposable

        public override void Dispose()
        {
            base.Dispose();

            Mask.Dispose();
            Mask = null;
        }

        #endregion
    }
}