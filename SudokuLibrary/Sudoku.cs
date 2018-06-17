using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Drawing;
using SudokuLibrary.ComputerVision;
using Emgu.CV.CvEnum;

namespace SudokuLibrary
{
    public class Sudoku
    {
        private Image<Bgr, Byte> Field;

        public Cell[,] Matrix { get; private set; }

        public int Size { get; private set; }


        public Sudoku(Bitmap image, int size = 9)
        {
            Size = size;
            Matrix = new Cell[Size, Size];

            Field = new GameFieldRecognizer(image).Recognize();
            Matrix = CellValueRecognizer.RecognizeDigits(Field, Size);
            Matrix = new SudokuSolver().Calculate(Matrix);
        }

        // Drawing preset and calculation values on the Field.
        public static void DrawDigits(Image<Bgr, Byte> field, Cell[,] matrix)
        {
            var FONT = Properties.Settings.Default.FONT;
            var FONTSIZE = Properties.Settings.Default.FONTSIZE;
            var FONTSIZEPR = Properties.Settings.Default.FONTSIZEPR;

            for (int yi = 0; yi < matrix.GetLength(0); yi++)
            {
                for (int xi = 0; xi < matrix.GetLength(0); xi++)
                {
                    if(matrix[xi, yi].Value == 0)
                    {
                        continue;
                    }

                    var leftBottom = new Point(matrix[xi, yi].Rect.Left, matrix[xi, yi].Rect.Bottom);

                    if (matrix[xi, yi].Preset)
                    {
                        // draw preset values
                        field.Draw(matrix[xi, yi].Rect, new Bgr(Color.Red), 1);
                        field.Draw(matrix[xi, yi].Value.ToString(), leftBottom, FONT, FONTSIZEPR, new Bgr(Color.Red));
                    }
                    else
                    {
                        // draw calculated values
                        field.Draw(matrix[xi, yi].Value.ToString(), leftBottom, FONT, FONTSIZE, new Bgr(Color.Green));
                    }
                }
            }
        }

        public Bitmap GetResultImage()
        {
            DrawDigits(Field, Matrix);

            return Field.ToBitmap();
        }

        public Bitmap GetLightImageResult()
        {
            const int SIZE = 271;
            const int KVADRANT = SIZE / 9;
            var resultImage = new Image<Bgr, byte>(SIZE, SIZE, new Bgr(Color.White));


            // Drawing field on the empty image
            for (int i = 0; i <= SIZE; i += KVADRANT)
            {
                int thickness = 1;
                if (i % (KVADRANT * 3) == 0 || i == 0 || i == SIZE - 1)
                    thickness = 2;

                Point[] points = new Point[] { new Point(0, i), new Point(SIZE, i) };
                resultImage.DrawPolyline(points, false, new Bgr(Color.Black), thickness);

                points = new Point[] { new Point(i, 0), new Point(i, SIZE) };
                resultImage.DrawPolyline(points, false, new Bgr(Color.Black), thickness);
            }

            // Drawing digits on the image
            for (int yi = 0; yi < Size; yi++)
            {
                for (int xi = 0; xi < Size; xi++)
                {
                    var leftBottom = new Point(xi * KVADRANT, (yi + 1) * KVADRANT);

                    if (Matrix[xi, yi].Preset)
                    {
                        // drawing preset values
                        resultImage.Draw(Matrix[xi, yi].Value.ToString(), leftBottom, FontFace.HersheyPlain, 2, new Bgr(Color.Black));
                    }
                    else
                    {
                        // drawing calculated values
                        resultImage.Draw(Matrix[xi, yi].Value.ToString(), leftBottom, FontFace.HersheyPlain, 2, new Bgr(Color.Green));
                    }
                }
            }

            return resultImage.ToBitmap();
        }
    }
}
