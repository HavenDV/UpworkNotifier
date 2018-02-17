using System.Windows;
using System.Windows.Media;
using UpworkNotifier.Utilities;

namespace UpworkNotifier.Windows
{
    public partial class SettingsWindow
    {
        #region Properties

        public SettingsWindow()
        {
            InitializeComponent();

            Update();
        }

        #endregion

        #region Event handlers

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            var path = DialogUtilities.OpenFileDialog();
            if (string.IsNullOrWhiteSpace(path))
            {
                return;
            }

            ModuleManager.Install(path);

            Update();
        }

        #endregion

        #region Event handlers

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            ModuleManager.Save();

            DialogResult = true;
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        #endregion

        #region Private methods

        private void Update()
        {
            var modules = ModuleManager.ActiveModules;

            ModulesPanel.Children.Clear();
            foreach (var module in modules)
            {
                var control = new Controls.ObjectControl(module.GetType().Name, module.Description)
                {
                    Height = 25,
                    Color = module.IsValid() ? Colors.LightGreen : Colors.Bisque
                };
                control.Deleted += (sender, args) =>
                {
                    ModuleManager.Deinstall(module);
                    Update();
                };
                control.Edited += (sender, args) =>
                {
                    var window = new ModuleSettingsWindow(module);
                    window.ShowDialog();
                    Update();
                };
                ModulesPanel.Children.Add(control);
            }
        }

        #endregion

    }
}
