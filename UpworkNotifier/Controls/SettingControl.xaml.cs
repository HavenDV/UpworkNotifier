using System;
using System.Windows.Media;

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

                UpdateColor();
            }
        }

        public Type Type { get; }
        public Func<string, object, bool> CheckFunc { get; }

        public string KeyName => $"{Key}({Type})";

        #endregion

        #region Constructors

        public SettingControl(string key, object value, Func<string, object, bool> checkFunc)
        {
            Key = key ?? throw new ArgumentNullException(nameof(key));
            _value = value ?? throw new ArgumentNullException(nameof(value));
            Type = value.GetType();
            CheckFunc = checkFunc ?? throw new ArgumentNullException(nameof(checkFunc));

            InitializeComponent();

            UpdateColor();
        }

        public void UpdateColor()
        {
            var isValid = CheckFunc.Invoke(Key, Value);
            TextBox.Background = new SolidColorBrush(isValid ? Colors.LightGreen : Colors.Bisque);
        }

        #endregion
    }
}
