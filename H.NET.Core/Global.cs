using System;
using System.Linq;

namespace H.NET.Core
{
    public static class Global
    {
        public static Func<string, ITarget> GetTargetByNameFunc { get; set; }
        public static ITarget GetTargetByName(string name) => GetTargetByNameFunc?.Invoke(name);

        public static void GenerateGetTargetByNameFuncFromModules(Func<IModule[]> func)
        {
            GetTargetByNameFunc = name => func?.Invoke().FirstOrDefault(i => i is ITarget target && target.Name == name) as ITarget;
        }
    }
}
