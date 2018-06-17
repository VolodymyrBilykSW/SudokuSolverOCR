using System.Drawing;

namespace SudokuLibrary.Extensions
{
    public static class DrawingExtension
    {
        public static Point[] ToPoints(this PointF[] pointfs)
        {
            var points = new Point[pointfs.Length];
            
            for (int i = 0; i < pointfs.Length; i++)
            {
                points[i] = Point.Round(pointfs[i]);
            }

            return points;
        }

        public static Point[] CorrectOrder(this Point[] points)
        {
            // 1--2
            // |  |
            // 4--3

            Point[] corners = new Point[4];

            int maxSum = 0, maxDiff = 0;
            int minSum = -1, minDiff = 0;

            foreach (var point in points)
            {
                var sum = point.X + point.Y;
                var diff = point.X - point.Y;

                // get right-bottom point
                if (sum > maxSum)
                {
                    corners[2] = point;
                    maxSum = sum;
                }

                // get left-top point
                if (sum < minSum || minSum == -1)
                {
                    corners[0] = point;
                    minSum = sum;
                }

                // get right-top point
                if (diff > maxDiff)
                {
                    corners[1] = point;
                    maxDiff = diff;
                }

                // get left-bottom point
                if (diff < minDiff)
                {
                    corners[3] = point;
                    minDiff = diff;
                }
            }

            return corners;
        }
    }
}
