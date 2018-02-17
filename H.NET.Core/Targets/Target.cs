namespace H.NET.Core.Targets
{
    public class Target : Module, ITarget
    {
        public virtual void SendMessage(string text) {}
    }
}
