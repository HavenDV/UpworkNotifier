using System;
using System.Windows;
using System.Windows.Input;
using UpworkNotifier.Notifiers;
using UpworkNotifier.Targets;

namespace UpworkNotifier
{
    public partial class MainWindow
    {
        #region Properties

        public INotifier Notifier { get; set; } = new ScreenshotNotifier();
        public ITarget Target { get; set; } = new TelegramTarget();

        #endregion

        #region Constructors

        public MainWindow()
        {
            InitializeComponent();

            Notifier.AfterScreenshot += OnNotifierOnAfterScreenshot;
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

        private void OnNotifierOnAfterScreenshot(object sender, EventArgs args)
        {
            Log("Notifier send event AfterScreenshot");
            Target.SendMessage("Hello");
        }

        #endregion


        #region Private methods

        public void Log(string message)
        {
            Dispatcher.Invoke(() =>
            {
                LogTextBox.Text += $"{DateTime.Now:T}: {message}{Environment.NewLine}";
            });
        }

        #endregion
    }
}
