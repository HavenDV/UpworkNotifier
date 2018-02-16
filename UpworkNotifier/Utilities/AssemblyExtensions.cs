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

        public static string[] GetDllPaths(this Assembly assembly)
        {
            var list = new List<string> { assembly.Location };

            var folder = Path.GetDirectoryName(assembly.Location) ?? "";
            list.AddRange(assembly.GetReferencedAssemblies()
                .Select(i => i.Name)
                .Where(i => !i.StartsWith("System.") && !i.StartsWith("mscorlib"))
                .Select(i => Path.Combine(folder, i + ".dll")));

            return list.ToArray();
        }
    }
}
