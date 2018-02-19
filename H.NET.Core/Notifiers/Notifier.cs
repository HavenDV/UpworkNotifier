using System;
using System.Linq;

namespace H.NET.Core.Notifiers
{
    public class Notifier : Module, INotifier
    {
        #region Properties

        public string Target { get; set; }
        public string Message { get; set; }

        #endregion

        #region Events

        public event EventHandler AfterEvent;
        protected void OnEvent() => AfterEvent?.Invoke(this, EventArgs.Empty);

        #endregion

        #region Constructors

        public Notifier()
        {
            AddSetting("Target", o => Target = o, TargetIsValid, string.Empty);
            AddSetting("Message", o => Message = o, StringIsValid, string.Empty);

            AfterEvent += (sender, args) =>
            {
                Log($"{Name} AfterEvent");
                try
                {
                    GetTargetByName(Target)?.SendMessage(Message);
                }
                catch (Exception exception)
                {
                    Log($"Exception: {exception}");
                }
            };
        }

        public bool StringIsValid(string path) => !string.IsNullOrWhiteSpace(path);

        public bool TargetIsValid(string target) => string.IsNullOrWhiteSpace(target) || !string.IsNullOrWhiteSpace(target) && GetTargetByName(target) != null;

        #endregion

        #region Static methods
        
        public static Func<string, ITarget> GetTargetByNameFunc { get; set; }
        public static ITarget GetTargetByName(string name) => GetTargetByNameFunc?.Invoke(name);

        public static void GenerateGetTargetByNameFuncFromModules(Func<IModule[]> func)
        {
            GetTargetByNameFunc = name => func?.Invoke()?.FirstOrDefault(i => i is ITarget target && target.Name == name) as ITarget;
        }

        #endregion

    }
}
