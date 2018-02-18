using System;

namespace H.NET.Core.Targets
{
    public abstract class Target : Module, ITarget
    {
        public void SendMessage(string text)
        {
            try
            {
                SendMessageInternal(text);
            }
            catch (Exception exception)
            {
                Log(exception.ToString());
            }
        }

        protected abstract void SendMessageInternal(string text);
    }
}
