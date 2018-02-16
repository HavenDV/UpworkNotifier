using System;
using System.Collections.Generic;
using System.ComponentModel;
using H.NET.Core.Storages;

namespace H.NET.Core
{
    public class Module : IModule
    {
        #region Properties

        public string Name { get; }
        public string Description { get; } = string.Empty;

        public ISettingsStorage Settings { get; } = new SettingsStorage();
        public virtual bool IsValid() => true;

        private Dictionary<string, Func<object, bool>> CheckSettingsDictionary { get; } = new Dictionary<string, Func<object, bool>>();
        private Dictionary<string, Action<object>> SetSettingsDictionary { get; } = new Dictionary<string, Action<object>>();

        #endregion

        #region Constructors

        protected Module()
        {
            Name = GetType().FullName;

            Settings.PropertyChanged += OnPropertyChanged;
        }

        #endregion

        #region Private/protected methods

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            var key = args.PropertyName;
            var value = Settings.TryGetValue(key, out var result) ? result : null;
            PropertyChanged(key, value);
        }

        protected void AddSetting(string key, Action<object> action, Func<object, bool> checkFunc = null, object defaultValue = null)
        {
            key = key ?? throw new ArgumentNullException(nameof(key));

            SetSettingsDictionary[key] = action;
            CheckSettingsDictionary[key] = checkFunc;
            Settings[key] = defaultValue;
        }

        protected void PropertyChanged(string key, object value)
        {
            if (!SetSettingsDictionary.ContainsKey(key))
            {
                return;
            }

            if (CheckSettingsDictionary.ContainsKey(key) && CheckSettingsDictionary[key]?.Invoke(value) != true)
            {
                return;
            }

            SetSettingsDictionary[key]?.Invoke(value);
        }

        #endregion
    }
}
