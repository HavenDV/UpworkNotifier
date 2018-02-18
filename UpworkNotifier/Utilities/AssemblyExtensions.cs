using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace UpworkNotifier.Utilities
{
    public static class AssemblyExtensions
    {
        public static Type[] GetTypesOfInterface<T>(this Assembly assembly) =>
            assembly.GetExportedTypes().Where(typeof(T).IsAssignableFrom).ToArray();

        public static T[] GetObjectsOfInterface<T>(this Assembly assembly) =>
            assembly.GetTypesOfInterface<T>().Select(Activator.CreateInstance).Cast<T>().ToArray();

        public static string GetFolder(this Assembly assembly) => Path.GetDirectoryName(assembly.Location) ?? ".";

        public static string[] GetDllPaths(this Assembly assembly)
        {
            var list = new List<string> { assembly.Location };

            var folder = assembly.GetFolder();
            list.AddRange(assembly.GetReferencedAssemblies()
                .Select(i => i.CodeBase ?? Path.Combine(folder, i.Name + ".dll"))
                .Where(i => !Path.GetFileName(i).StartsWith("System.") &&
                            !Path.GetFileName(i).StartsWith("mscorlib")));

            list.AddRange(GetFilesIfExists(Path.Combine(folder, "x86")));
            list.AddRange(GetFilesIfExists(Path.Combine(folder, "x64")));

            return list.ToArray();
        }

        private static string[] GetFilesIfExists(string folder) =>
            Directory.Exists(folder) 
                ? Directory.EnumerateFiles(folder).ToArray()
                : new string[0];
    }
}
