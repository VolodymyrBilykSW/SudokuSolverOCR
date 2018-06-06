using System;
using System.Drawing;
using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.CvEnum;
using Emgu.CV.Util;

namespace SudokuLibrary.ComputerVision
{
    public class GameFieldRecognizer
    {
        public static Image<Bgr, Byte> Recognize(string path2photo)
        {
            // Max size for input image, if it`s more then resize image
            int MAXSIZE = Properties.Settings.Default.MAXSIZE; //2000;

            // Size for output image, recommendation: multiples of 9 and 6
            int RSIZE = Properties.Settings.Default.RSIZE; //360;

            var CHAINAPPROX = Properties.Settings.Default.CHAINAPPROX;
            var L2GRADIENT = Properties.Settings.Default.L2Gradient;


            // Load the image from file and resize it
            var photo = new Image<Bgr, Byte>(path2photo);
            if (photo.Width > MAXSIZE && photo.Height > MAXSIZE)
                photo = photo.Resize(MAXSIZE, MAXSIZE * photo.Width / photo.Height, Inter.Linear, true);

            // Convert the image to grayscale and filter out the noise
            var uimage = new UMat();
            CvInvoke.CvtColor(photo, uimage, ColorConversion.Bgr2Gray);

            // Use image pyr to remove noise
            var pyrDown = new UMat();
            CvInvoke.PyrDown(uimage, pyrDown);
            CvInvoke.PyrUp(pyrDown, uimage);



            var cannyEdges = new UMat();
            CvInvoke.Canny(uimage, cannyEdges, 50.0, 100.0, l2Gradient: L2GRADIENT);

            // Another methods to process image, but worse. Use only one!
            //CvInvoke.Threshold(uimage, cannyEdges, 50.0, 100.0, ThresholdType.Binary);
            //CvInvoke.AdaptiveThreshold(uimage, cannyEdges, 50, AdaptiveThresholdType.MeanC, ThresholdType.Binary, 7, 1);

            double maxRectArea = 0;
            var biggestRectangle = new PointF[4];

            using (var contours = new VectorOfVectorOfPoint())
            {
                // Finding contours and choosing needed
                CvInvoke.FindContours(cannyEdges, contours, null, RetrType.List, CHAINAPPROX);

                for (int i = 0; i < contours.Size; i++)
                {
                    if (contours[i].Size < 4)
                        continue;

                    var shape = Get4CornerPoints(contours[i].ToArray());
                    if (IsRectangle(shape))
                    {
                        var rect = CvInvoke.MinAreaRect(shape);
                        var area = rect.Size.Height * rect.Size.Width;

                        if (area > maxRectArea)
                        {
                            maxRectArea = area;
                            biggestRectangle = shape;
                        }
                    }
                }
            }


            var resultField = new Image<Bgr, byte>(RSIZE, RSIZE);
            PointF[] newCorners = { new PointF(0, 0), new PointF(RSIZE, 0), new PointF(0, RSIZE), new PointF(RSIZE, RSIZE) };

            // Transformation sudoky field to rectangle size and aligning the sides
            var M = CvInvoke.GetPerspectiveTransform(biggestRectangle, newCorners);
            CvInvoke.WarpPerspective(photo, resultField, M, new Size(RSIZE, RSIZE));

            return resultField;
        }


        // Getting four corner points from contour points
        private static PointF[] Get4CornerPoints(Point[] points)
        {
            // 1--2
            // |  |
            // 3--4

            PointF[] corners = new PointF[4];

            Int32 maxSum = 0, maxDiff = 0;
            Int32 minSum = -1, minDiff = 0;

            foreach (var point in points)
            {
                var sum = point.X + point.Y;
                var diff = point.X - point.Y;

                // get right-bottom point
                if (sum > maxSum)
                {
                    corners[3] = point;
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
                    corners[2] = point;
                    minDiff = diff;
                }
            }

            return corners;
        }


        // Get true if contour is rectangle with angles within [lowAngle, upAngle] degree. Default: [75, 105]
        private static Boolean IsRectangle(PointF[] contour, Int32 lowAngle = 75, Int32 upAngle = 105, Double ratio = 0.35)
        {
            if (contour.Length > 4)
                return false;

            LineSegment2DF[] sides = { new LineSegment2DF(contour[0], contour[1]),
                                       new LineSegment2DF(contour[1], contour[3]),
                                       new LineSegment2DF(contour[2], contour[3]),
                                       new LineSegment2DF(contour[0], contour[2]) };

            // Check angles between common sides.
            for (int j = 0; j < 4; j++)
            {
                double angle = Math.Abs(sides[(j + 1) % sides.Length].GetExteriorAngleDegree(sides[j]));

                if (angle < lowAngle || angle > upAngle)
                    return false;
            }

            // Check ratio between all sides, it can`t be more than allowed.
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    if (sides[i].Length / sides[j].Length < ratio || sides[i].Length / sides[j].Length > (1 + ratio))
                        return false;
                }
            }

            return true;
        }
    }
}
