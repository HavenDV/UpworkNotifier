using System.Drawing;
using System.IO;
using Emgu.CV;
using H.NET.Core;
using H.NET.Notifiers.Extensions;
using H.NET.Notifiers.Utilities;

namespace H.NET.Notifiers
{
    public class UpworkScreenshotNotifier : ScreenshotTimerNotifier
    {
        #region Properties

        private string _examplePath;
        public string ExamplePath
        {
            get => _examplePath;
            set
            {
                _examplePath = value;

                if (!IsValid())
                {
                    return;
                }

                Mask = new Mat(ExamplePath).ToGray();
            }
        }

        private Mat Mask { get; set; }

        public string Target { get; set; }
        public string Message { get; set; }

        #endregion

        #region Contructors

        public UpworkScreenshotNotifier()
        {
            AddSetting("ExamplePath", o => ExamplePath = o, PathIsValid, string.Empty);
            AddSetting("Target", o => Target = o, TargetIsValid, string.Empty);
            AddSetting("Message", o => Message = o, StringIsValid, string.Empty);

            AfterEvent += (sender, args) => Global.GetTargetByName(Target)?.SendMessage(Message);
        }

        #endregion

        #region Protected methods

        public bool PathIsValid(string path) => !string.IsNullOrWhiteSpace(path) && File.Exists(path);

        public bool StringIsValid(string path) => !string.IsNullOrWhiteSpace(path);

        public bool TargetIsValid(string target) => !string.IsNullOrWhiteSpace(target) && Global.GetTargetByName(target) != null;

        protected override bool Analyze(Bitmap bitmap)
        {
            using (var mat = bitmap.ToMat())
            using (var grayMat = mat.ToGray())
            {
                return ScreenshotUtilities.IsEquals(grayMat, Mask, Mask);
            }
        }

        #endregion

        #region IDisposable

        public override void Dispose()
        {
            base.Dispose();

            Mask.Dispose();
            Mask = null;
        }

        #endregion
    }
}