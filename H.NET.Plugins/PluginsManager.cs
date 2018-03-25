using System;
using System.IO;
using System.Linq;
using System.Reflection;
using H.NET.Plugins.Extensions;
using H.NET.Plugins.Utilities;

namespace H.NET.Plugins
{
    public class PluginsManager<T> : AssembliesManager
    {
        #region Properties

        public const string SettingsSubFolder = "Settings";
        public const string SettingsExtension = ".json";

        public Action<T, string> LoadAction { get; }
        public Func<T, string> SaveFunc { get; }

        public string SettingsFolder { get; }
        public T[] ActivePlugins { get; private set; }
        public Type[] AvailableTypes { get; private set; }

        #endregion

        #region Constructors

        public PluginsManager(string companyName, Action<T, string> loadAction, Func<T, string> saveFunc) : base(companyName)
        {
            LoadAction = loadAction;
            SaveFunc = saveFunc;

            SettingsFolder = DirectoryUtilities.CombineAndCreateDirectory(BaseFolder, SettingsSubFolder);
        }

        #endregion

        #region Public methods

        #region Load/Save

        public override void Load()
        {
            base.Load();

            if (ActivePlugins != null)
            {
                foreach (var plugin in ActivePlugins.Where(i => i is IDisposable).Cast<IDisposable>())
                {
                    plugin.Dispose();
                }
            }

            ActivePlugins = ActiveAssemblies.SelectMany(LoadPluginsFromAssembly).ToArray();
            AvailableTypes = ActiveAssemblies.SelectMany(i => i.GetTypesOfInterface<T>()).ToArray();
        }

        public override void Save()
        {
            base.Save();

            foreach (var plugin in ActivePlugins ?? new T[0])
            {
                SavePluginSettings(plugin);
            }
        }

        #endregion


        #endregion

        #region Private methods

        #region Load/Save Settings

        private string GetDefaultSettingsPath(T plugin) => Path.Combine(SettingsFolder, plugin.GetType().FullName + SettingsExtension);

        private void LoadPluginSettings(T plugin, string path = null)
        {
            path = path ?? GetDefaultSettingsPath(plugin);

            if (string.IsNullOrWhiteSpace(path) || !File.Exists(path))
            {
                return;
            }

            var text = File.ReadAllText(path);
            LoadAction?.Invoke(plugin, text);
        }

        private void SavePluginSettings(T plugin, string path = null)
        {
            var text = SaveFunc?.Invoke(plugin);
            if (text == null)
            {
                return;
            }

            path = path ?? GetDefaultSettingsPath(plugin);
            File.WriteAllText(path, text);
        }

        #endregion

        private T[] LoadPluginsFromAssembly(Assembly assembly)
        {
            try
            {
                var plugins = assembly.GetObjectsOfInterface<T>();
                foreach (var plugin in plugins)
                {
                    LoadPluginSettings(plugin);
                }

                return plugins;
            }
            catch (Exception)
            {
                return new T[0];
            }
        }

        #endregion
    }
}
