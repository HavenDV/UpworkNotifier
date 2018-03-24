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

            ModuleManager.Instance.Install(path);

            Update();
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            ModuleManager.Instance.Save();

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
            var assemblies = ModuleManager.Instance.ActiveAssemblies;

            AssembliesPanel.Children.Clear();
            foreach (var assembly in assemblies)
            {
                var control = new Controls.ObjectControl(assembly.GetName().Name)
                {
                    Height = 25,
                    Color = Colors.LightGreen,
                    EnableEditing = false,
                    EnableAdding = false
                };
                control.Deleted += (sender, args) =>
                {
                    ModuleManager.Instance.Deinstall(assembly);
                    Update();
                };
                AssembliesPanel.Children.Add(control);
            }

            var availableModules = ModuleManager.Instance.ActivePlugins;

            AvailableTypesPanel.Children.Clear();
            foreach (var module in availableModules)
            {
                var control = new Controls.ObjectControl(module.Name)
                {
                    Height = 25,
                    Color = module.IsValid() ? Colors.LightGreen : Colors.Bisque
                };
                control.Deleted += (sender, args) =>
                {
                    ModuleManager.Instance.Deinstall(module);
                    Update();
                };
                control.Edited += (sender, args) =>
                {
                    var window = new ModuleSettingsWindow(module);
                    window.ShowDialog();
                    Update();
                };
                control.Added += (sender, args) =>
                {
                    var window = new ModuleSettingsWindow(module);
                    window.ShowDialog();
                    Update();
                };
                AvailableTypesPanel.Children.Add(control);
            }
        }

        #endregion

    }
}
