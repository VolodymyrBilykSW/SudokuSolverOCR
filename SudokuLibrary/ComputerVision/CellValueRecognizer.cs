using System;
using System.IO;
using Emgu.CV;
using Emgu.CV.OCR;
using Emgu.CV.Structure;
using Emgu.CV.CvEnum;
using System.Drawing;

namespace SudokuLibrary.ComputerVision
{
    public class CellValueRecognizer
    {
        private static Tesseract _ocr;


        public static void InitTesseract(string dir, string lang = "eng")
        {
            try
            {
                if (!File.Exists(dir + $"\\{lang}.traineddata"))
                    TesseractDownloadLangFile(dir, lang);
            }
            catch (Exception)
            {
                throw new InvalidOperationException("Tessaract Error. Don`t have a file and can`t to download it.");
            }

            _ocr = new Tesseract(dir, lang, OcrEngineMode.TesseractOnly, "123456789");
        }


        private static Cell[,] RecognizeDigits(Image<Bgr, Byte> field, int size)
        {
            var matrix = new Cell[size, size];

            int width = field.Width / size;
            int offset = width / 6;

            for (int yi = 0; yi < size; yi++)
            {
                for (int xi = 0; xi < size; xi++)
                {
                    // get image from centre cell
                    matrix[xi, yi].Rect = new Rectangle(offset + width * xi, offset + width * yi, width - offset * 2, width - offset * 2);

                    // recognize digit from cell
                    int digit = CellValueRecognizer.Recognize(field.GetSubRect(matrix[xi, yi].Rect));

                    if (digit == 0)
                        continue;

                    if (!matrix.IsPossible(xi, yi, digit))
                        throw new Exception($"Recognition error. Don`t possible value at cell [{xi},{yi}]");

                    matrix[xi, yi].Value = digit;
                    matrix[xi, yi].Preset = true;
                }
            }

            return matrix;
        }

        public static int Recognize(Image<Bgr, Byte> cellImg)
        {
            // Convert the image to grayscale and filter out the noise
            Mat imgGrey = new Mat();
            CvInvoke.CvtColor(cellImg, imgGrey, ColorConversion.Bgr2Gray);

            // TODO: can be problem with values for some image
            Mat imgThresholded = new Mat();
            CvInvoke.Threshold(imgGrey, imgThresholded, 170, 255, ThresholdType.Binary);

            _ocr.SetImage(imgThresholded);

            if (_ocr.Recognize() != 0)
            {
                throw new InvalidOperationException("Tessaract Error. Can`t to recognize cell image");
            }

            Tesseract.Character[] characters = _ocr.GetCharacters();
            int digit = 0;
            string number = "";

            foreach (var c in characters)
            {
                if (c.Text != " ")
                {
                    number += c.Text;
                }
            }

            if (number.Length > 1)
            {
                return 0;
            }

            Int32.TryParse(number, out digit);

            return digit;
        }

        // ----------------------------------------------------------------------
        private static void TesseractDownloadLangFile(String dir, String lang)
        {
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            using (System.Net.WebClient webclient = new System.Net.WebClient())
            {
                String Source = String.Format($"https://github.com/tesseract-ocr/tessdata/raw/master/{lang}.traineddata");
                String Dest = String.Format(dir + $"\\{lang}.traineddata");

                webclient.DownloadFileAsync(new Uri(Source), Dest);
            }
        }
    }
}
