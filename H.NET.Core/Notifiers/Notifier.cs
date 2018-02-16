using System;

namespace H.NET.Core.Notifiers
{
    public class Notifier : Module, INotifier
    {
        #region Events

        public event EventHandler AfterEvent;
        protected void OnEvent() => AfterEvent?.Invoke(this, EventArgs.Empty);

        #endregion

        #region IDisposable

        public virtual void Dispose()
        {
        }

        #endregion
    }
}
