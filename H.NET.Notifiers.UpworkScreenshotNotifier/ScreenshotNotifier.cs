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

        private string _examplePath;
        public string ExamplePath
        {
            get => _examplePath;
            set
            {
                _examplePath = value;
                Mask = new Mat(ExamplePath).ToGray();
            }
        }

        private Mat Mask { get; set; } 

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