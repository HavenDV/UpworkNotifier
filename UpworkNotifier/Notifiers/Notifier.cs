using System;

namespace UpworkNotifier.Notifiers
{
    public class Notifier : INotifier
    {
        public event EventHandler AfterScreenshot;
        protected void OnScreenshot() => AfterScreenshot?.Invoke(this, EventArgs.Empty);
    }
}
