using Ookii.Dialogs.Wpf;
using System;
using System.IO;
using System.Windows;
using UpworkNotifier.Notifiers;
using UpworkNotifier.Properties;
using UpworkNotifier.Targets;

namespace UpworkNotifier
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
            LoadSettings();

            Notifier.AfterScreenshot += OnNotifierOnAfterScreenshot;
            Settings.Default.SettingChanging += (o, args) => SaveButton.IsEnabled = true;
        }

        private void LoadSettings()
        {
            // Dispose object if exists
            Dispose();

            // Load Screenshot Notifier
            Notifier = new ScreenshotNotifier(Settings.Default.ScreenshotExamplePath, Settings.Default.ScreenshotInterval);

            // Load Telegram Target
            var token = Settings.Default.TelegramToken.Trim();
            var userId = Settings.Default.TelegramUserId;
            if (string.IsNullOrWhiteSpace(token) || userId <= 0)
            {
                Log(Properties.Resources.Failed_to_load_Telegram_Target_Token_or_UserId_is_invalid);
                return;
            }
            Target = new TelegramTarget(token, userId);

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

        private void ScreenshotExamplePathButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new VistaFolderBrowserDialog
            {
                ShowNewFolderButton = true
            };

            if (Directory.Exists(ScreenshotExamplePathTextBox.Text))
            {
                dialog.SelectedPath = ScreenshotExamplePathTextBox.Text + @"\";
            }

            var result = dialog.ShowDialog();
            if (result != true)
            {
                return;
            }

            Settings.Default.ScreenshotExamplePath = dialog.SelectedPath;
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

        #endregion

        #region Private methods

        private void Log(string message) => Dispatcher.Invoke(() => LogTextBox.Text += $"{DateTime.Now:T}: {message}{Environment.NewLine}");

        #endregion
    }
}
