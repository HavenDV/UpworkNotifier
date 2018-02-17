namespace H.NET.Core
{
    public interface ITarget : IModule
    {
        void SendMessage(string message);
    }
}
