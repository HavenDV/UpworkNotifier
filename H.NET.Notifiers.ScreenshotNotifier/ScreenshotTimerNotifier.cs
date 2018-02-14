using System.Drawing;

// ReSharper disable once CheckNamespace
namespace H.NET.Notifiers
{
    public abstract class ScreenshotTimerNotifier : TimerNotifier
    {
        #region Constructors

        protected ScreenshotTimerNotifier(int interval) : base(interval)
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
