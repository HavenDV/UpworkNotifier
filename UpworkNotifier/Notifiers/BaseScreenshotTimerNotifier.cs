﻿using Emgu.CV;
using UpworkNotifier.Extensions;
using UpworkNotifier.Notifiers.Core;
using UpworkNotifier.Utilities;

namespace UpworkNotifier.Notifiers
{
    public abstract class BaseScreenshotTimerNotifier : BaseTimerNotifier
    {
        #region Constructors

        protected BaseScreenshotTimerNotifier(int interval) : base(interval)
        {
        }

        #endregion

        #region Private methods

        protected abstract bool Analyze(Mat mat);

        protected override async void OnElapsed()
        {
            using (var image = await Screenshoter.ShotAsync())
            using (var bitmap = image.ToBitmap())
            using (var mat = bitmap.ToMat())
            {
                if (Analyze(mat))
                {
                    OnEvent();
                }
            }
        }

        #endregion

    }
}
