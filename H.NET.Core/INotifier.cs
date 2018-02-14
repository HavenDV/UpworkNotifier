using System;

namespace H.NET.Core
{
    public interface INotifier : IDisposable
    {
        event EventHandler AfterEvent;
    }
}
