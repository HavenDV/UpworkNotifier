using System;
using System.IO;
using System.Linq;
using System.Reflection;
using H.NET.Plugins.Extensions;
using H.NET.Plugins.Utilities;

namespace H.NET.Plugins
{
    public class PluginsManager<T>
    {
        #region Properties

        public string CompanyName { get; }
        public Action<T, string> LoadAction { get; }
        public Func<T, string> SaveFunc { get; }

        public string AppDataFolder { get; } = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        public string BaseFolder { get; }
        public string ModulesFolder { get; }
        public string SettingsFolder { get; }

        public string ActiveFolder { get; private set; } = string.Empty;
        public T[] ActiveModules { get; private set; }
        public Assembly[] ActiveAssemblies { get; private set; }

        #endregion

        #region Constructors

        public PluginsManager(string companyName, Action<T, string> loadAction, Func<T, string> saveFunc)
        {
            CompanyName = companyName ?? throw new ArgumentNullException(nameof(companyName));
            LoadAction = loadAction;
            SaveFunc = saveFunc;

            BaseFolder = DirectoryUtilities.CombineAndCreateDirectory(AppDataFolder, CompanyName);
            ModulesFolder = DirectoryUtilities.CombineAndCreateDirectory(BaseFolder, "Modules");
            SettingsFolder = DirectoryUtilities.CombineAndCreateDirectory(BaseFolder, "Settings");
        }

        #endregion

        #region Public methods

        #region Load/Save

        public void Load()
        {
            TryClean();

            ActiveFolder = CreateActiveFolder();
            DirectoryUtilities.CopyDirectory(ModulesFolder, ActiveFolder);

            if (ActiveModules != null)
            {
                foreach (var module in ActiveModules.Where(i => i is IDisposable).Cast<IDisposable>())
                {
                    module.Dispose();
                }
            }

            ActiveAssemblies = Directory
                .EnumerateDirectories(ActiveFolder)
                .Select(folder => Path.Combine(folder, $"{Path.GetFileName(folder) ?? ""}.dll"))
                .Where(i => !string.IsNullOrWhiteSpace(i))
                .Where(File.Exists)
                .Select(LoadAssembly)
                .ToArray();

            ActiveModules = ActiveAssemblies.SelectMany(LoadModulesFromAssembly).ToArray();
        }

        public void Save()
        {
            TryClean();

            foreach (var module in ActiveModules ?? new T[0])
            {
                SaveModuleSettings(module);
            }
        }

        #endregion

        #region Install/Deinstall

        public void Install(Assembly assembly)
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

        public void Install(string path) => Install(Assembly.LoadFile(path));

        public void Deinstall(Assembly assembly)
        {
            TryClean();

            var toFolder = GetModuleFolder(assembly);
            Directory.Delete(toFolder, true);

            Load();
        }

        public void Deinstall(Type type) => Deinstall(type.Assembly);
        public void Deinstall(T module) => Deinstall(module.GetType());

        #endregion

        #endregion

        #region Private methods

        private string CreateActiveFolder() => DirectoryUtilities.CombineAndCreateDirectory(BaseFolder, $"ActiveModules_{new Random().Next()}");
        private string GetModuleFolder(Assembly assembly) => DirectoryUtilities.CombineAndCreateDirectory(ModulesFolder, assembly.GetName().Name);
        private string GetDefaultSettingsPath(T module) => Path.Combine(SettingsFolder, module.GetType().FullName + ".json");

        #region Load/Save Settings

        private void LoadModuleSettings(T module, string path = null)
        {
            path = path ?? GetDefaultSettingsPath(module);

            if (string.IsNullOrWhiteSpace(path) || !File.Exists(path))
            {
                return;
            }

            var text = File.ReadAllText(path);
            LoadAction?.Invoke(module, text);
        }

        private void SaveModuleSettings(T module, string path = null)
        {
            var text = SaveFunc?.Invoke(module);
            if (text == null)
            {
                return;
            }

            path = path ?? GetDefaultSettingsPath(module);
            File.WriteAllText(path, text);
        }

        #endregion

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

        private static Assembly LoadAssembly(string path)
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


            return Assembly.LoadFrom(path);
        }

        private T[] LoadModulesFromAssembly(Assembly assembly)
        {
            try
            {
                var modules = assembly.GetObjectsOfInterface<T>();
                foreach (var module in modules)
                {
                    LoadModuleSettings(module);
                }

                return modules;
            }
            catch (Exception)
            {
                return new T[0];
            }
        }

        private void TryClean()
        {
            foreach (var directory in Directory.EnumerateDirectories(BaseFolder, "ActiveModules_*"))
            {
                if (string.Equals(ActiveFolder, directory, StringComparison.OrdinalIgnoreCase))
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

        #endregion
    }
}
