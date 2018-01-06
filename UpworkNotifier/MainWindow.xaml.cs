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

        private INotifier Notifier { get; set; } = new ScreenshotNotifier(Settings.Default.ScreenshotExamplePath, Settings.Default.ScreenshotInterval);
        private ITarget Target { get; } = new TelegramTarget(Settings.Default.TelegramToken.Trim(), Settings.Default.TelegramUserId);

        #endregion

        #region Constructors

        public MainWindow()
        {
            InitializeComponent();

            Notifier.AfterScreenshot += OnNotifierOnAfterScreenshot;
            Settings.Default.SettingChanging += (o, args) => SaveButton.IsEnabled = true;
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
        }

        private void OnNotifierOnAfterScreenshot(object sender, EventArgs args)
        {
            Log("Notifier send event AfterScreenshot");

            var message = Settings.Default.Message;
            Target.SendMessage(string.IsNullOrWhiteSpace(message) 
                ? Properties.Resources.Message_is_empty__Please_set_the_message_in_the_General_Settings
                : message);
        }

        #endregion

        #region Private methods

        private void Log(string message) => Dispatcher.Invoke(() => LogTextBox.Text += $"{DateTime.Now:T}: {message}{Environment.NewLine}");

        #endregion
    }
}
