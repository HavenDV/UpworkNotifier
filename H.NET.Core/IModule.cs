namespace H.NET.Core
{
    public interface IModule
    {
        string Name { get; }
        string Description { get; }

        ISettingsStorage Settings { get; }
        bool IsValid();
    }
}
