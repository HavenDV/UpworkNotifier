using System.Timers;
using H.NET.Core.Notifiers;

namespace H.NET.Notifiers
{
    public abstract class TimerNotifier : Notifier
    {
        #region Properties

        private int _interval;
        public int Interval
        {
            get => _interval;
            set
            {
                _interval = value;

                if (!IsValid())
                {
                    return;
                }

                Timer?.Dispose();
                Timer = new Timer(value);
                Timer.Elapsed += OnElapsed;
                Timer.Start();
            }
        }

        private Timer Timer { get; set; }

        #endregion

        #region Events

        #endregion

        #region Constructors

        protected TimerNotifier()
        {
            AddSetting("Interval", o => Interval = (int)o, o => o is int, int.MaxValue);
        }

        public override bool IsValid() => base.IsValid() && Interval > 0;

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
