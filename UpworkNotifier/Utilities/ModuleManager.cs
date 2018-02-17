using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace UpworkNotifier.Utilities
{
    public static class ModuleManager
    {
        #region Properties

        public static string AppDataFolder { get; } = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        public static string BaseFolder { get; } = CombineAndCreate(AppDataFolder, "H.NET");
        public static string GetModuleFolder(Assembly assembly) => CombineAndCreate(BaseFolder, assembly.GetName().Name);

        #endregion

        #region Public methods

        public static Assembly[] Load() => Directory
            .EnumerateDirectories(BaseFolder)
            .Select(folder => Path.Combine(folder, $"{Path.GetFileName(folder) ?? ""}.dll"))
            .Where(i => !string.IsNullOrWhiteSpace(i))
            .Where(File.Exists)
            .Select(Assembly.LoadFile)
            .ToArray();

        public static void Install(Assembly assembly)
        {
            var toFolder = GetModuleFolder(assembly);
            var files = assembly.GetDllPaths();

            foreach (var path in files)
            {
                var name = Path.GetFileName(path) ?? throw new Exception($"Path is not valid");
                File.Copy(path, Path.Combine(toFolder, name));
            }
        }

        public static void Deinstall(Assembly assembly)
        {
            var toFolder = GetModuleFolder(assembly);
            Directory.Delete(toFolder, true);
        }

        #endregion

        #region Private methods

        private static string CombineAndCreate(params string[] arguments) =>
            Directory.CreateDirectory(Path.Combine(arguments)).FullName;

        #endregion
    }
}
