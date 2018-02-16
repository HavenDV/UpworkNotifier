using System;
using System.IO;
using System.Windows;
using H.NET.Core;
using H.NET.Notifiers;
using H.NET.Targets;
using UpworkNotifier.Properties;
using UpworkNotifier.Utilities;

namespace UpworkNotifier.Windows
{
    public partial class MainWindow : IDisposable
    {
        #region Properties

        private INotifier Notifier { get; set; }
        private ITarget Target { get; set; }

        #endregion

        #region Constructors

        public MainWindow()
        {
            InitializeComponent();

            Visibility = Visibility.Hidden;

            LoadSettings();

            Settings.Default.SettingChanging += (o, args) => SaveButton.IsEnabled = true;
        }

        private void LoadSettings()
        {
            // Dispose object if exists
            Dispose();

            var errors = false;

            // Load Screenshot Notifier
            var path = Settings.Default.ScreenshotExamplePath;
            var interval = Settings.Default.ScreenshotInterval;
            if (File.Exists(path) && interval > 0)
            {
                Notifier = new UpworkScreenshotNotifier{ ExamplePath = path, Interval = interval };
                Notifier.AfterEvent += OnNotifierOnAfterScreenshot;
            }
            else
            {
                Log(Properties.Resources.Failed_to_load_Screenshot_Notifier__Path_or_Interval_is_invalid);
                errors = true;
            }

            // Load Telegram Target
            var token = Settings.Default.TelegramToken.Trim();
            var userId = Settings.Default.TelegramUserId;
            if (!string.IsNullOrWhiteSpace(token) && userId > 0)
            {
                Target = new TelegramTarget{ Token = token, UserId = userId };
            }
            else
            {
                Log(Properties.Resources.Failed_to_load_Telegram_Target_Token_or_UserId_is_invalid);
                errors = true;
            }

            if (errors)
            {
                return;
            }

            // Show message
            Log(Properties.Resources.Settings_successfully_loaded);
        }

        #endregion

        #region IDisposable

        public void Dispose()
        {
            Notifier?.Dispose();
            Notifier = null;
        }

        #endregion

        #region Event handlers

        private void ShowHide_Click(object sender, RoutedEventArgs e)
        {
            if (Visibility == Visibility.Visible)
            {
                Hide();
            }
            else
            {
                Show();
            }
        }

        private void Close_Click(object sender, RoutedEventArgs e) => Close();

        private void ScreenshotExamplePathButton_Click(object sender, RoutedEventArgs e)
        {
            var path = DialogUtilities.OpenFileDialog(ScreenshotExamplePathTextBox.Text);
            if (string.IsNullOrWhiteSpace(path))
            {
                return;
            }

            Settings.Default.ScreenshotExamplePath = path;
            Settings.Default.Save();
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            Settings.Default.Save();
            SaveButton.IsEnabled = false;

            LoadSettings();
        }

        private void OnNotifierOnAfterScreenshot(object sender, EventArgs args)
        {
            Log("Notifier send event AfterScreenshot");

            var message = Settings.Default.Message;
            Target?.SendMessage(string.IsNullOrWhiteSpace(message)
                ? Properties.Resources.Message_is_empty__Please_set_the_message_in_the_General_Settings
                : message);
        }

        private void Settings_Click(object sender, RoutedEventArgs e) => new SettingsWindow().ShowDialog();

        #endregion

        #region Private methods

        private void Log(string message) => Dispatcher.Invoke(() => LogTextBox.Text += $"{DateTime.Now:T}: {message}{Environment.NewLine}");
        
        #endregion
    }
}
