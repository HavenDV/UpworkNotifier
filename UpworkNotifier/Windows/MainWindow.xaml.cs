﻿using System;
using System.Windows;
using H.NET.Core;
using H.NET.Core.Notifiers;
using UpworkNotifier.Utilities;

namespace UpworkNotifier.Windows
{
    public partial class MainWindow
    {
        #region Constructors

        public MainWindow()
        {
            InitializeComponent();

            Visibility = Visibility.Hidden;

            Module.LogAction = Log;
            Notifier.RunAction = command => Log($"Command run: {command}");

            Log("Loading modules...");
            try
            {
                ModuleManager.Instance.Load();

                Log("Loaded");
            }
            catch (Exception exception)
            {
                Log(exception.ToString());
            }
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
        
        private void Settings_Click(object sender, RoutedEventArgs e) => new SettingsWindow().ShowDialog();

        #endregion

        #region Private methods

        private void Log(string message) => Dispatcher.Invoke(() => LogTextBox.Text += $"{DateTime.Now:T}: {message}{Environment.NewLine}");
        
        #endregion
    }
}
