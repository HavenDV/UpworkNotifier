using System;
using System.Linq;
using System.Reflection;

namespace UpworkNotifier.Utilities
{
    public static class AssemblyExtensions
    {
        public static object[] GetObjectsOfInterface(this Assembly assembly, Type type) =>
            GetTypesOfInterface(assembly, type).Select(Activator.CreateInstance).ToArray();

        public static Type[] GetTypesOfInterface(this Assembly assembly, Type type) =>
            assembly.GetExportedTypes().Where(type.IsAssignableFrom).ToArray();

        public static T[] GetObjectsOfInterface<T>(this Assembly assembly) =>
            GetTypesOfInterface(assembly, typeof(T)).Select(Activator.CreateInstance).Cast<T>().ToArray();
    }
}
