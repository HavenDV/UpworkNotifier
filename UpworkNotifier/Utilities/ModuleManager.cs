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

        public static IModule[] ActiveModules { get; private set; }

        public static string GetActiveModulesFolder() => CombineAndCreate(BaseFolder, $"ActiveModules_{new Random().Next()}");
        public static string GetModuleFolder(Assembly assembly) => CombineAndCreate(ModulesFolder, assembly.GetName().Name);
        private static string GetDefaultSettingsPath(IModule module) =>
            Path.Combine(SettingsFolder, module.GetType().FullName + ".json");

        #endregion

        #region Public methods
        /*
        public class ProxyDomain : MarshalByRefObject
        {
            public Assembly GetAssembly(string path)
            {
                try
                {
                    return Assembly.LoadFile(path);
                }
                catch (Exception exception)
                {
                    throw new InvalidOperationException(exception.Message, exception);
                }
            }
        }
        */
        private static IModule[] LoadAssembly(string path)
        {
            try
            {
                /*
                var domain = AppDomain.CreateDomain("H.NET.Plugins.Domain", AppDomain.CurrentDomain.Evidence, new AppDomainSetup
                {
                    ApplicationBase = Path.GetDirectoryName(path) ?? Environment.CurrentDirectory
                });

                domain.UnhandledException += (sender, args) =>
                    MessageBox.Show(((Exception) args.ExceptionObject).ToString());
                    */

                //var type = typeof(Proxy);
                //var value = (Proxy)domain.CreateInstanceAndUnwrap(
                //    type.Assembly.FullName,
                //    type.FullName);

                //var assembly = value.GetAssembly(path);
                //var module = Domain.CreateInstanceFromAndUnwrap(path, type.Name) as IModule;

                var assembly = Assembly.LoadFrom(path);
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
            var dictionary = JsonConvert.DeserializeObject<Dictionary<string, CoreSetting>>(text);
            foreach (var pair in dictionary)
            {
                module.Settings.CopyFrom(pair.Key, pair.Value);
            }
        }

        public static void SaveModuleSettings(IModule module, string path = null)
        {
            var dictionary = module.Settings.ToDictionary(entry => entry.Key, entry => entry.Value.Copy());
            var text = JsonConvert.SerializeObject(dictionary);

            path = path ?? GetDefaultSettingsPath(module);
            File.WriteAllText(path, text);
        }

        public static void CopyDirectory(string fromFolder, string toFolder) => Directory
            .GetFiles(fromFolder, "*.*", SearchOption.AllDirectories)
            .ToList()
            .ForEach(fromPath =>
            {
                var toPath = fromPath.Replace(fromFolder, toFolder);
                Directory.CreateDirectory(Path.GetDirectoryName(toPath) ?? "");

                File.Copy(fromPath, toPath, true);
            });

        public static void Load()
        {
            TryClean();

            CurrentActiveModulesFolder = GetActiveModulesFolder();
            CopyDirectory(ModulesFolder, CurrentActiveModulesFolder);

            if (ActiveModules != null)
            {
                foreach (var module in ActiveModules)
                {
                    module.Dispose();
                }
            }
            ActiveModules = Directory
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

            foreach (var module in ActiveModules ?? new IModule[0])
            {
                SaveModuleSettings(module);
            }
        }

        public static void Install(Assembly assembly)
        {
            TryClean();

            var toFolder = GetModuleFolder(assembly);
            var fromFolder = assembly.GetFolder();
            var paths = assembly.GetDllPaths();
            
            foreach (var path in paths)
            {
                var directory = Path.GetFileName(Path.GetDirectoryName(path));
                var name = Path.GetFileName(path) ?? throw new Exception("Invalid file name");
                if (string.Equals(directory, "x86", StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(directory, "x64", StringComparison.OrdinalIgnoreCase))
                {
                    name = Path.Combine(directory, name);
                }

                var fromPath = Path.Combine(fromFolder, name);
                var toPath = Path.Combine(toFolder, name);

                Directory.CreateDirectory(Path.GetDirectoryName(toPath) ?? "");
                File.Copy(fromPath, toPath, true);
            }

            Load();
        }

        public static void Install(string path) => Install(Assembly.LoadFile(path));

        public static void Deinstall(Assembly assembly)
        {
            TryClean();
            
            var toFolder = GetModuleFolder(assembly);
            Directory.Delete(toFolder, true);

            Load();
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
