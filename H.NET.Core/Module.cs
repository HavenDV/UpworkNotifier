using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using H.NET.Core.Storages;

namespace H.NET.Core
{
    public class Module : IModule
    {
        #region Properties

        public string Name { get; }
        public string Description { get; } = string.Empty;

        public ISettingsStorage Settings { get; } = new SettingsStorage();
        public bool IsValid() => Settings.All(entry => SettingIsValid(entry.Key, entry.Value));

        private Dictionary<string, Action<object>> SetSettingsDictionary { get; } = new Dictionary<string, Action<object>>();
        private Dictionary<string, Func<object, bool>> CheckSettingsDictionary { get; } = new Dictionary<string, Func<object, bool>>();

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

        protected void AddSetting<T>(string key, Action<T> action, Func<T, bool> checkFunc, T defaultValue)
        {
            key = key ?? throw new ArgumentNullException(nameof(key));

            SetSettingsDictionary[key] = o => action?.Invoke(ConvertTo<T>(o));
            CheckSettingsDictionary[key] = o => CanConvert<T>(o) && checkFunc?.Invoke(ConvertTo<T>(o)) == true;
            Settings[key] = defaultValue;
        }

        private bool CanConvert<T>(object value)
        {
            try
            {
                var unused = Convert.ChangeType(value, typeof(T));
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private T ConvertTo<T>(object value) => (T)Convert.ChangeType(value, typeof(T));

        public bool SettingIsValid(string key, object value)
        {
            key = key ?? throw new ArgumentNullException(nameof(key));

            return CheckSettingsDictionary.ContainsKey(key) && CheckSettingsDictionary[key]?.Invoke(value) == true;
        }

        private void PropertyChanged(string key, object value)
        {
            if (!SetSettingsDictionary.ContainsKey(key))
            {
                return;
            }

            if (!SettingIsValid(key, value))
            {
                return;
            }

            SetSettingsDictionary[key]?.Invoke(value);
        }

        #endregion

        #region Static methods

        public static Action<string> LogAction { get; set; }
        public static void Log(string text) => LogAction?.Invoke(text);

        #endregion
    }
}
