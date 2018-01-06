using System;
using System.Timers;
using Emgu.CV;
using UpworkNotifier.Extensions;
using WpfUtilities.Extensions;
using WpfUtilities.Utilities;

namespace UpworkNotifier.Notifiers
{
    public class ScreenshotNotifier : Notifier, IDisposable
    {
        #region Properties

        public Timer Timer { get; set; } = new Timer(1000);

        #endregion

        #region Constructors

        public ScreenshotNotifier()
        {
            Timer.Elapsed += OnElapsed;
            Timer.Start();
        }

        #endregion

        #region IDisposable

        public void Dispose()
        {
            Timer.Stop();
            Timer?.Dispose();
            Timer = null;
        }

        #endregion

        #region Private methods

        private async void OnElapsed(object sender, ElapsedEventArgs e)
        {
            var image = await Screenshoter.ShotAsync();
            var mat = image.ToBitmap().ToMat();

            if (Analyze(mat))
            {
                OnScreenshot();
            }
        }

        private bool Analyze(Mat mat)
        {
            return true;
        }

        #endregion

    }
}
