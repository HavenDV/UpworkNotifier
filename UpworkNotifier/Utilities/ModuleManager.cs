﻿using System.Collections.Generic;
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
                var list = JsonConvert.DeserializeObject<List<CoreSetting>>(text);
                foreach (var value in list)
                {
                    module.Settings.CopyFrom(value.Key, value);
                }
            }, module =>
            {
                var list = module.Settings.Select(i => i.Value.Copy()).ToList();

                return JsonConvert.SerializeObject(list, Formatting.Indented);
            });

        #endregion
    }
}
