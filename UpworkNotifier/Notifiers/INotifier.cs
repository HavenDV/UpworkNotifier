using System;

namespace UpworkNotifier.Notifiers
{
    public interface INotifier : IDisposable
    {
        event EventHandler AfterEvent;
    }
}
