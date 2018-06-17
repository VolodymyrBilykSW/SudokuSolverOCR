using Microsoft.Win32;
using System.Windows;
using SudokuLibrary;
using SudokuLibrary.WPF;
using SudokuLibrary.ComputerVision;
using System.Drawing;
using System.IO;

namespace SudokuWPF
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            try
            {
                var path = Directory.GetCurrentDirectory() + @"\tessdata";
                CellValueRecognizer.InitTesseract(path);
            }
            catch (System.Exception error)
            {
                MessageBox.Show($"Can`t to calculate\nSource: {error.Source}\nMessage: {error.Message}",
                                "Fatal error",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
                throw;
            }
        }

        private void RecognizeAndSolve_Click(object sender, RoutedEventArgs e)
        {
            var openImage = new OpenFileDialog() { Filter = "Image|*.BMP;*.JPG;*.GIF;*.PNG|All files|*.*" };
            openImage.ShowDialog();

            if (openImage.FileName != "")
            {
                try
                {
                    var bmp = new Bitmap(openImage.FileName);
                    var resImg = new Sudoku(bmp).GetResultImage();

                    var resultWin = new ImageViewer() { Title = "Answer" };
                    resultWin.image.Source = resImg.ToImageSource();
                    resultWin.Show();
                }
                catch (System.Exception error)
                {
                    MessageBox.Show($"Can`t to calculate\nSource: {error.Source}\nMessage: {error.Message}",
                                    "Error",
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Error);
                }
            }
        }


        private void GetField_Click(object sender, RoutedEventArgs e)
        {
            var openImage = new OpenFileDialog() { Filter = "Image|*.BMP;*.JPG;*.GIF;*.PNG|All files|*.*" };
            openImage.ShowDialog();


            if (openImage.FileName != "")
            {
                try
                {
                    var field = GameFieldRecognizer.Recognize(new Bitmap(openImage.FileName));

                    var fieldWin = new ImageViewer() { Title = "Game field" };
                    fieldWin.image.Source = field.ToBitmapSource();
                    fieldWin.Show();
                }
                catch (System.Exception error)
                {
                    MessageBox.Show($"Can`t get game field\nSource: {error.Source}\nMessage: {error.Message}",
                                    "Error",
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Error);
                }
            }
        }

        private void SettingsMenu_Click(object sender, RoutedEventArgs e)
        {
            new SettingsWindow().Show();
        }

        private void AboutMenu_Click(object sender, RoutedEventArgs e)
        {

        }

        private void OpenSudokuBoardBut_Click(object sender, RoutedEventArgs e)
        {
            new Views.SudokuBoard().Show();
        }

        private void DemonstrationBut_Click(object sender, RoutedEventArgs e)
        {
            var openImage = new OpenFileDialog() { Filter = "Image|*.BMP;*.JPG;*.GIF;*.PNG|All files|*.*" };
            openImage.ShowDialog();


            if (openImage.FileName != "")
            {
                try
                {
                    var bmp = new Bitmap(openImage.FileName);

                    var field = GameFieldRecognizer.TestRecognize(bmp);

                    var fieldWin = new ImageViewer() { Title = "Game field" };
                    fieldWin.image.Source = field.ToBitmapSource();
                    fieldWin.Show();
                }
                catch (System.Exception error)
                {
                    MessageBox.Show($"Can`t get game field\nSource: {error.Source}\nMessage: {error.Message}",
                                    "Error",
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Error);
                }
            }
        }
    }
}
