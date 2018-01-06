using System;
using System.Timers;
using Emgu.CV;
using UpworkNotifier.Extensions;
using WpfUtilities.Extensions;
using WpfUtilities.Utilities;

namespace UpworkNotifier.Notifiers.Core
{
    public class BaseScreenshotTimerNotifier : BaseNotifier
    {
        #region Properties

        public Timer Timer { get; set; }
        public Func<Mat, bool> AnalyzeFunc { get; set; }

        #endregion

        #region Constructors

        public BaseScreenshotTimerNotifier(int interval)
        {
            Timer = new Timer(interval);
            Timer.Elapsed += OnElapsed;
            Timer.Start();
        }

        #endregion

        #region IDisposable

        public new void Dispose()
        {
            base.Dispose();

            Timer.Stop();
            Timer?.Dispose();
            Timer = null;
        }

        #endregion

        #region Private methods

        private async void OnElapsed(object sender, ElapsedEventArgs e)
        {
            if (AnalyzeFunc == null)
            {
                return;
            }

            using (var image = await Screenshoter.ShotAsync())
            using (var bitmap = image.ToBitmap())
            using (var mat = bitmap.ToMat())
            {
                if (AnalyzeFunc(mat))
                {
                    OnScreenshot();
                }
            }
        }

        #endregion

    }
}
