using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Media;
using H.NET.Core;
using Newtonsoft.Json;
using UpworkNotifier.Utilities;

namespace UpworkNotifier.Windows
{
    public partial class SettingsWindow
    {
        #region Properties

        public List<IModule> Modules { get; } = new List<IModule>();

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

            var modules = LoadAssembly(path);
            Modules.AddRange(modules);

            Update();
        }

        #endregion

        #region Event handlers

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            Save();

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
            ModulesPanel.Children.Clear();
            foreach (var module in Modules)
            {
                var control = new Controls.ObjectControl(module.Name, module.Description) { Height = 25 };
                control.Color = module.IsValid() ? Colors.GreenYellow : Colors.Red;
                control.Deleted += (sender, args) =>
                {
                    //Storage.Remove(key);
                    Update();
                };
                control.Edited += (sender, args) =>
                {
                    var window = new ModuleSettingsWindow(module.Settings);
                    window.ShowDialog();
                    Update();
                };
                ModulesPanel.Children.Add(control);
            }
        }

        private void Save()
        {
            foreach (var module in Modules)
            {
                SaveModuleSettings(module);
            }
        }

        #endregion

        private static IModule[] LoadAssembly(string path)
        {
            var assembly = Assembly.LoadFile(path);
            var modules = assembly.GetObjectsOfInterface<IModule>();
            foreach (var module in modules)
            {
                LoadModuleSettings(module);
            }

            return modules;
        }

        private static string GetDefaultSettingsPath(IModule module) =>
            Path.Combine(Path.GetDirectoryName(module.GetType().Assembly.Location) ?? "", module.GetType().FullName + ".json");

        private static void LoadModuleSettings(IModule module, string path = null)
        {
            path = path ?? GetDefaultSettingsPath(module);

            if (string.IsNullOrWhiteSpace(path) || !File.Exists(path))
            {
                return;
            }

            var text = File.ReadAllText(path);
            var dictionary = JsonConvert.DeserializeObject<Dictionary<string, object>>(text);
            foreach (var pair in dictionary)
            {
                module.Settings[pair.Key] = pair.Value;
            }
        }

        private static void SaveModuleSettings(IModule module, string path = null)
        {
            var dictionary = module.Settings.ToDictionary(entry => entry.Key, entry => entry.Value);
            var text = JsonConvert.SerializeObject(dictionary);

            path = path ?? GetDefaultSettingsPath(module);
            File.WriteAllText(path, text);
        }

    }
}
