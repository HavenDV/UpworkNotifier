using System;

namespace H.NET.Core
{
    public interface INotifier : IModule, IDisposable
    {
        event EventHandler AfterEvent;
    }
}
