using System.Timers;
using Emgu.CV;
using UpworkNotifier.Extensions;
using UpworkNotifier.Utilities;

namespace UpworkNotifier.Notifiers.Core
{
    public abstract class BaseScreenshotTimerNotifier : BaseNotifier
    {
        #region Properties

        public Timer Timer { get; set; }

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
        
        public override void Dispose()
        {
            base.Dispose();

            Timer.Stop();
            Timer?.Dispose();
            Timer = null;
        }

        #endregion

        #region Private methods

        protected abstract bool Analyze(Mat mat);

        private async void OnElapsed(object sender, ElapsedEventArgs e)
        {
            using (var image = await Screenshoter.ShotAsync())
            using (var bitmap = image.ToBitmap())
            using (var mat = bitmap.ToMat())
            {
                if (Analyze(mat))
                {
                    OnScreenshot();
                }
            }
        }

        #endregion

    }
}
