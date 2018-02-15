using System.Timers;
using H.NET.Core.Notifiers;

// ReSharper disable once CheckNamespace
namespace H.NET.Notifiers
{
    public abstract class TimerNotifier : BaseNotifier
    {
        #region Properties

        private int _interval;
        public int Interval
        {
            get => _interval;
            set
            {
                _interval = value;

                Timer?.Dispose();
                Timer = new Timer(value);
                Timer.Elapsed += OnElapsed;
                Timer.Start();
            }
        }

        protected Timer Timer { get; set; }

        #endregion

        #region Events

        #endregion

        #region Constructors

        protected TimerNotifier()
        {
            Interval = int.MaxValue;
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
