using System.Drawing;
using System.IO;
using Emgu.CV;
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

        #endregion

        #region Contructors

        public UpworkScreenshotNotifier()
        {
            AddSetting("ExamplePath", o => ExamplePath = o as string, o => o is string, string.Empty);
        }

        #endregion

        #region Protected methods

        public override bool IsValid() => base.IsValid() && !string.IsNullOrWhiteSpace(ExamplePath) && File.Exists(ExamplePath);

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