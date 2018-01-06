using System;
using Emgu.CV;

namespace UpworkNotifier.Utilities
{
    public static class ScreenshotUtilities
    {
        public static double Epsilon { get; set; } = 0.001;

        public static double GetDifference(Mat mat1, Mat mat2, Mat mask = null)
        {
            if (mask != null && !mask.IsEmpty)
            {
                CvInvoke.BitwiseOr(mat1, mat1, mat1, mask);
                CvInvoke.BitwiseOr(mat2, mat2, mat2, mask);
            }

            var foreground = new Mat();
            CvInvoke.AbsDiff(mat1, mat2, foreground);

            return CvInvoke.Mean(foreground).V0;
        }

        public static bool IsEquals(double difference) => Math.Abs(difference) < Epsilon;
        public static bool IsEquals(Mat mat1, Mat mat2, Mat mask = null) => IsEquals(GetDifference(mat1, mat2, mask));
    }
}
