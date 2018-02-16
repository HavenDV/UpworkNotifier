using System;

namespace UpworkNotifier.Controls
{
    public partial class SettingControl
    {
        #region Properties

        public string Key { get; }

        private object _value;
        public object Value
        {
            get => _value;
            set
            {
                try
                {
                    _value = Convert.ChangeType(value, Type);
                }
                catch (Exception)
                {
                    // ignored
                }
            }
        }

        public Type Type { get; }

        public string KeyName => $"{Key}({Type})";

        #endregion

        #region Constructors

        public SettingControl(string key, object value)
        {
            Key = key ?? throw new ArgumentNullException(nameof(key));
            _value = value ?? throw new ArgumentNullException(nameof(value));
            Type = value.GetType();

            InitializeComponent();
        }

        #endregion
    }
}
