namespace UpworkNotifier.Targets
{
    public interface ITarget
    {
        void SendMessage(string message);
    }
}
