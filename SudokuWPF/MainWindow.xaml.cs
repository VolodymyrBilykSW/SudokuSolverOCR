using Microsoft.Win32;
using System.Windows;
using SudokuLibrary;
using SudokuLibrary.WPF;
using SudokuLibrary.ComputerVision;
using System.Drawing;

namespace SudokuWPF
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            try
            {
                CellValueRecognizer.InitTesseract();
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

        private void RecognizePhoto_Click(object sender, RoutedEventArgs e)
        {
            var openImage = new OpenFileDialog() { Filter = "Image|*.BMP;*.JPG;*.GIF;*.PNG|All files|*.*" };
            openImage.ShowDialog();

            if (openImage.FileName != "")
            {
                try
                {
                    var bmp = new Bitmap(openImage.FileName);
                    var resImg = new Sudoku(bmp).GetResultImage();

                    var resultWin = new ImageViewer() { Title = "Game field" };
                    resultWin.image.Source = resImg.ToBitmap();
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
                    var field = GameFieldRecognizer.Recognize(openImage.FileName);

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
            var settWin = new SettingsWindow();
            settWin.Show();
        }
    }
}
