using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using H.NET.Core;
using Newtonsoft.Json;

namespace UpworkNotifier.Utilities
{
    public static class ModuleManager
    {
        #region Properties

        public static string AppDataFolder { get; } = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        public static string BaseFolder { get; } = CombineAndCreate(AppDataFolder, "H.NET");
        public static string ModulesFolder { get; } = CombineAndCreate(BaseFolder, "Modules");
        public static string SettingsFolder { get; } = CombineAndCreate(BaseFolder, "Settings");
        public static string CurrentActiveModulesFolder { get; private set; } = string.Empty;

        public static IModule[] ActiveModules { get; private set; } = Load();

        public static string GetActiveModulesFolder() => CombineAndCreate(BaseFolder, $"ActiveModules_{new Random().Next()}");
        public static string GetModuleFolder(Assembly assembly) => CombineAndCreate(ModulesFolder, assembly.GetName().Name);
        private static string GetDefaultSettingsPath(IModule module) =>
            Path.Combine(SettingsFolder, module.GetType().FullName + ".json");

        #endregion

        #region Public methods

        private static IModule[] LoadAssembly(string path)
        {
            try
            {
                var assembly = Assembly.LoadFile(path);
                var modules = assembly.GetObjectsOfInterface<IModule>();
                foreach (var module in modules)
                {
                    LoadModuleSettings(module);
                }

                return modules;
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.ToString());
                return new IModule[0];
            }
        }

        public static void TryClean()
        {
            foreach (var directory in Directory.EnumerateDirectories(BaseFolder, "ActiveModules_*"))
            {
                if (string.Equals(CurrentActiveModulesFolder, directory, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                try
                {
                    Directory.Delete(directory, true);
                }
                catch (Exception)
                {
                    // ignored
                }
            }
        }

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

        public static void SaveModuleSettings(IModule module, string path = null)
        {
            var dictionary = module.Settings.ToDictionary(entry => entry.Key, entry => entry.Value);
            var text = JsonConvert.SerializeObject(dictionary);

            path = path ?? GetDefaultSettingsPath(module);
            File.WriteAllText(path, text);
        }

        public static string[] ModuleFolders => Directory.EnumerateDirectories(ModulesFolder).ToArray();

        public static void CopyDirectory(string fromFolder, string toFolder) => Directory
            .GetFiles(fromFolder, "*.*", SearchOption.AllDirectories)
            .ToList()
            .ForEach(fromPath =>
            {
                var toPath = fromPath.Replace(fromFolder, toFolder);
                Directory.CreateDirectory(Path.GetDirectoryName(toPath) ?? "");

                File.Copy(fromPath, toPath, true);
            });

        public static IModule[] Load()
        {
            TryClean();

            CurrentActiveModulesFolder = GetActiveModulesFolder();
            CopyDirectory(ModulesFolder, CurrentActiveModulesFolder);

            return Directory
                .EnumerateDirectories(CurrentActiveModulesFolder)
                .Select(folder => Path.Combine(folder, $"{Path.GetFileName(folder) ?? ""}.dll"))
                .Where(i => !string.IsNullOrWhiteSpace(i))
                .Where(File.Exists)
                .SelectMany(LoadAssembly)
                .ToArray();
        }

        public static void Save()
        {
            TryClean();

            foreach (var module in ActiveModules)
            {
                SaveModuleSettings(module);
            }
        }

        public static void Install(Assembly assembly)
        {
            TryClean();

            var toFolder = GetModuleFolder(assembly);
            var files = assembly.GetDllPaths();
            
            foreach (var path in files)
            {
                var name = Path.GetFileName(path) ?? throw new Exception("Path is not valid");
                File.Copy(path, Path.Combine(toFolder, name), true);
            }

            ActiveModules = Load();
        }

        public static void Install(string path) => Install(Assembly.LoadFile(path));

        public static void Deinstall(Assembly assembly)
        {
            TryClean();

            ActiveModules = null;

            var toFolder = GetModuleFolder(assembly);
            Directory.Delete(toFolder, true);

            ActiveModules = Load();
        }

        public static void Deinstall(Type type) => Deinstall(type.Assembly);
        public static void Deinstall(IModule module) => Deinstall(module.GetType());

        #endregion

        #region Private methods

        private static string CombineAndCreate(params string[] arguments) =>
            Directory.CreateDirectory(Path.Combine(arguments)).FullName;

        #endregion
    }
}
