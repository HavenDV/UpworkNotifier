using System.Drawing;
using H.NET.Notifiers.ScreenshotNotifier.Utilities;

namespace H.NET.Notifiers.ScreenshotNotifier
{
    public abstract class BaseScreenshotTimerNotifier : BaseTimerNotifier
    {
        #region Constructors

        protected BaseScreenshotTimerNotifier(int interval) : base(interval)
        {
        }

        #endregion

        #region Private methods

        protected abstract bool Analyze(Bitmap bitmap);

        protected override async void OnElapsed()
        {
            using (var image = await Screenshoter.ShotAsync())
            using (var bitmap = new Bitmap(image))
            {
                if (Analyze(bitmap))
                {
                    OnEvent();
                }
            }
        }

        #endregion

    }
}
