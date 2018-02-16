using System.Collections.Generic;
using System.ComponentModel;

namespace H.NET.Core.Storages
{
    public class SettingsStorage : Dictionary<string, object>, ISettingsStorage
    {
        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        public new object this[string key]
        {
            get => base[key];
            set
            {
                base[key] = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(key));
            }
        }

        public new bool Remove(string key)
        {
            var result = base.Remove(key);
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(key));

            return result;
        }

        #endregion

        public void Load() { }
        public void Save() { }
    }
}
