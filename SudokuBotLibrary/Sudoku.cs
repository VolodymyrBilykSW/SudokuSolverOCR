using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Drawing;
using SudokuBotLibrary.ComputerVision;

namespace SudokuBotLibrary
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
        }


        public Image<Bgr, Byte> GetResultImage()
        {
            RecognizeDigits();
            Matrix = new SudokuSolver().Calculate(Matrix);
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
    }
}
