using System.ComponentModel;

namespace H.NET.Core
{
    public interface ISettingsStorage : IStorage<object>, INotifyPropertyChanged
    {
    }
}
