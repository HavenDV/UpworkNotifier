using System;

namespace UpworkNotifier.Notifiers.Core
{
    public class BaseNotifier : INotifier
    {
        public event EventHandler AfterScreenshot;
        protected void OnScreenshot() => AfterScreenshot?.Invoke(this, EventArgs.Empty);

        #region IDisposable

        public void Dispose()
        {
        }

        #endregion
    }
}
