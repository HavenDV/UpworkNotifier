using System;

namespace UpworkNotifier.Notifiers.Core
{
    public class BaseNotifier : INotifier
    {
        #region Events

        public event EventHandler AfterScreenshot;
        protected void OnScreenshot() => AfterScreenshot?.Invoke(this, EventArgs.Empty);

        #endregion

        #region IDisposable

        public virtual void Dispose()
        {
        }

        #endregion
    }
}
