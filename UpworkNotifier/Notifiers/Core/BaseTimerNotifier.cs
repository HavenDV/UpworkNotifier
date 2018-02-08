using System.Timers;

namespace UpworkNotifier.Notifiers.Core
{
    public abstract class BaseTimerNotifier : BaseNotifier
    {
        #region Properties

        protected Timer Timer { get; set; }

        #endregion

        #region Constructors

        protected BaseTimerNotifier(int interval)
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

        #region Protected methods

        protected abstract void OnElapsed();

        private void OnElapsed(object sender, ElapsedEventArgs e) => OnElapsed();

        #endregion

    }
}
