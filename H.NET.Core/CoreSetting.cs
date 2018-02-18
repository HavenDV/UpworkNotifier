namespace H.NET.Core
{
    public class CoreSetting
    {
        public string Key { get; set; }
        public object Value { get; set; }
        public object DefaultValue { get; set; }
        public SettingType SettingType { get; set; }

        public void CopyTo(CoreSetting setting)
        {
            setting.Key = Key;
            setting.Value = Value;
            setting.DefaultValue = DefaultValue;
            setting.SettingType = SettingType;
        }

        public void CopyFrom(CoreSetting setting)
        {
            Key = setting.Key;
            Value = setting.Value;
            DefaultValue = setting.DefaultValue;
            SettingType = setting.SettingType;
        }

        public CoreSetting Copy()
        {
            var setting = new CoreSetting();
            setting.CopyFrom(this);

            return setting;
        }
    }
}
