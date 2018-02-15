using System;
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
    }
}
