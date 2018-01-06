using System;

namespace UpworkNotifier.Notifiers
{
    public interface INotifier
    {
        event EventHandler AfterScreenshot;
    }
}
