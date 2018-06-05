using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Drawing;
using SudokuLibrary.ComputerVision;
using Emgu.CV.CvEnum;

namespace SudokuLibrary
{
    class Sudoku
    {
        public Image<Bgr, Byte> Field { get; private set; }

        public Cell[,] Matrix { get; private set; }

        public int Size { get; private set; }


        public Sudoku(Image<Bgr, Byte> field, int size = 9)
        {
            Size = size;
            Field = field;
            Matrix = new Cell[Size, Size];

            RecognizeDigits();
            Matrix = new SudokuSolver().Calculate(Matrix);
        }


        public Image<Bgr, Byte> GetResultImage()
        {
            DrawDigits();

            return Field;
        }

        // Get digits from sudoky image field.
        private void RecognizeDigits()
        {
            int width = Field.Width / Size;
            int offset = width / 6;

            for (int yi = 0; yi < Size; yi++)
            {
                for (int xi = 0; xi < Size; xi++)
                {
                    // get image from centre cell
                    Matrix[xi, yi].Rect = new Rectangle(offset + width * xi, offset + width * yi, width - offset * 2, width - offset * 2);

                    // recognize digit from cell
                    int digit = CellValueRecognizer.Recognize(Field.GetSubRect(Matrix[xi, yi].Rect));

                    if (digit == 0)
                        continue;

                    //if (!Matrix.isPossible(xi, yi, digit))
                    //   throw new Exception("Recognition error. Don`t possible value");

                    Matrix[xi, yi].Value = digit;
                    Matrix[xi, yi].Preset = true;
                }
            }
        }

        // Drawing preset and calculation values on the Field.
        private void DrawDigits()
        {
            var FONT = Properties.Settings.Default.FONT;
            var FONTSIZE = Properties.Settings.Default.FONTSIZE;
            var FONTSIZEPR = Properties.Settings.Default.FONTSIZEPR;

            for (int yi = 0; yi < Size; yi++)
            {
                for (int xi = 0; xi < Size; xi++)
                {
                    var leftBottom = new Point(Matrix[xi, yi].Rect.Left, Matrix[xi, yi].Rect.Bottom);

                    if (Matrix[xi, yi].Preset)
                    {
                        // draw preset values
                        Field.Draw(Matrix[xi, yi].Rect, new Bgr(Color.Red), 1);
                        Field.Draw(Matrix[xi, yi].Value.ToString(), leftBottom, FONT, FONTSIZEPR, new Bgr(Color.Red));
                    }
                    else
                    {
                        // draw calculated values
                        Field.Draw(Matrix[xi, yi].Value.ToString(), leftBottom, FONT, FONTSIZE, new Bgr(Color.Green));
                    }
                }
            }
        }


        private Image<Bgr, byte> GetLightResult()
        {
            const int SIZE = 271;
            const int KVADRANT = SIZE / 9;
            var resultImage = new Image<Bgr, byte>(SIZE, SIZE, new Bgr(Color.White));


            // Drawing empty field
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

            // Drawing digits
            for (int yi = 0; yi < Size; yi++)
            {
                for (int xi = 0; xi < Size; xi++)
                {
                    var leftBottom = new Point(xi * KVADRANT, (yi + 1) * KVADRANT);

                    if (Matrix[xi, yi].Preset)
                    {
                        // draw preset values
                        resultImage.Draw(Matrix[xi, yi].Value.ToString(), leftBottom, FontFace.HersheyPlain, 2, new Bgr(Color.Black));
                    }
                    else
                    {
                        // draw calculated values
                        resultImage.Draw(Matrix[xi, yi].Value.ToString(), leftBottom, FontFace.HersheyPlain, 2, new Bgr(Color.Green));
                    }
                }
            }

            return resultImage;
        }
    }
}
