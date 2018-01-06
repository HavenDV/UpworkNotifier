using UpworkNotifier.Notifiers.Core;

namespace UpworkNotifier.Notifiers
{
    public class ScreenshotNotifier : BaseScreenshotTimerNotifier
    {
        #region Constructors

        public ScreenshotNotifier() : base(1000)
        {
            AnalyzeFunc = mat =>
            {
                return true;
            };
        }

        #endregion

    }
}
