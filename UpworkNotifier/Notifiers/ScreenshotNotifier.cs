using Emgu.CV;
using Emgu.CV.CvEnum;
using UpworkNotifier.Notifiers.Core;
using UpworkNotifier.Utilities;

namespace UpworkNotifier.Notifiers
{
    public class ScreenshotNotifier : BaseScreenshotTimerNotifier
    {
        #region Properties

        private string ExamplePath { get; }

        #endregion

        #region Constructors

        public ScreenshotNotifier(string examplePath, int interval) : base(interval)
        {
            ExamplePath = examplePath;
            AnalyzeFunc = Analyze;
        }

        private bool Analyze(Mat mat)
        {
            CvInvoke.CvtColor(mat, mat, ColorConversion.Bgra2Gray);

            var mask = new Mat(ExamplePath);
            CvInvoke.CvtColor(mask, mask, ColorConversion.Bgr2Gray);

            return ScreenshotUtilities.IsEquals(mat, mask, mask);
        }

        #endregion

    }
}
