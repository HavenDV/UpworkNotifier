using Emgu.CV;
using Emgu.CV.CvEnum;
using UpworkNotifier.Extensions;
using UpworkNotifier.Notifiers.Core;
using UpworkNotifier.Utilities;

namespace UpworkNotifier.Notifiers
{
    public class ScreenshotNotifier : BaseScreenshotTimerNotifier
    {
        #region Properties

        private string ExamplePath { get; }
        private Mat Mask { get; set; } 

        #endregion

        #region Constructors

        public ScreenshotNotifier(string examplePath, int interval) : base(interval)
        {
            ExamplePath = examplePath;

            Mask = new Mat(ExamplePath).ToGray();
        }

        #endregion

        #region BaseScreenshotTimerNotifier

        protected override bool Analyze(Mat mat)
        {
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
