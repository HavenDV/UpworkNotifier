using System.Collections.Generic;
using System.Linq;
using H.NET.Core;
using H.NET.Plugins;
using Newtonsoft.Json;

namespace UpworkNotifier.Utilities
{
    public static class ModuleManager
    {
        #region Properties

        public static PluginsManager<IModule> Instance { get; } = new PluginsManager<IModule>("H.NET",
            (module, text) =>
            {
                var dictionary = JsonConvert.DeserializeObject<Dictionary<string, CoreSetting>>(text);
                foreach (var pair in dictionary)
                {
                    module.Settings.CopyFrom(pair.Key, pair.Value);
                }
            }, module =>
            {
                var dictionary = module.Settings.ToDictionary(entry => entry.Key, entry => entry.Value.Copy());
                return JsonConvert.SerializeObject(dictionary);
            });

        #endregion
    }
}
