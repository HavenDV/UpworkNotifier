using UpworkNotifier.Notifiers.Core;

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
            AnalyzeFunc = mat => true;
        }

        #endregion

    }
}
