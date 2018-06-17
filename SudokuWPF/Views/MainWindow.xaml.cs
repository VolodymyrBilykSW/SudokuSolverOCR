using Microsoft.Win32;
using System.Windows;
using SudokuLibrary;
using SudokuLibrary.WPF;
using SudokuLibrary.ComputerVision;
using SudokuLibrary.Extensions;
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
                    var sudoku = new Sudoku(bmp);

                    var resultWin = new ImageViewer() { Title = $"Answer, solving for {sudoku.SolvingTime} ms" };
                    resultWin.image.Source = sudoku.GetResultImage().ToImageSource();
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
                    var field = new GameFieldRecognizer(new Bitmap(openImage.FileName)).Recognize();

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

            if (openImage.FileName == "")
                return;

            try
            {
                Views.DemonstrationWindow demWin;

                var bmp = new Bitmap(openImage.FileName);
                var gameRecognizer = new GameFieldRecognizer(bmp);

                // stage 1
                demWin = new Views.DemonstrationWindow() { Title = "Stage 1. Download Image" };
                demWin.image.Source = bmp.ToImageSource();
                demWin.ExplainBox.Text = "Photo which is needed to process and calculate";
                demWin.ShowDialog();

                // stage 2
                var preparedImage = gameRecognizer.PrepareImage();

                demWin = new Views.DemonstrationWindow() { Title = "Stage 2. Processing image" };
                demWin.image.Source = preparedImage.Bitmap.ToImageSource();
                demWin.ExplainBox.Text = "Photo after processing";
                demWin.ShowDialog();

                // stage 3
                var gameContour = gameRecognizer.FindField(preparedImage);
                
                demWin = new Views.DemonstrationWindow() { Title = "Stage 3. Finding field" };
                var img = gameRecognizer.Image.Clone();
                img.DrawPolyline(gameContour.ToPoints().CorrectOrder(), true, new Emgu.CV.Structure.Bgr(Color.Green), img.Size.Width/100);

                demWin.image.Source = img.ToBitmapSource();
                demWin.ExplainBox.Text = "Found game field in green rectangle";
                demWin.ShowDialog();

                // stage 4
                demWin = new Views.DemonstrationWindow() { Title = "Stage 4. Cutting field" };
                var gameFieldImg = gameRecognizer.CutField(gameContour);
                demWin.image.Source = gameFieldImg.Bitmap.ToImageSource();
                demWin.ExplainBox.Text = "Cut game field from original photo";
                demWin.ShowDialog();

                // stage 5
                var sudoku = CellValueRecognizer.RecognizeDigits(gameFieldImg);

                demWin = new Views.DemonstrationWindow() { Title = "Stage 5. Recognizing digits" };
                Sudoku.DrawDigits(gameFieldImg, sudoku);
                demWin.image.Source = gameFieldImg.Bitmap.ToImageSource();
                demWin.ExplainBox.Text = "Recognized and then painted digits on the game field";
                demWin.ShowDialog();

                // stage 6
                var solver = new SudokuSolver(sudoku);

                bool run = true;
                while (run)
                {
                    run = solver.FindOnePossible();

                    if (run == false)
                        run = solver.FindOnlyHere();
                }
                
                demWin = new Views.DemonstrationWindow() { Title = "Stage 6. Solving sudoku" };
                Sudoku.DrawDigits(gameFieldImg, solver.Matrix);
                demWin.image.Source = gameFieldImg.Bitmap.ToImageSource();
                demWin.ExplainBox.Text = $"Solved unknown sudoku values using simple methods";
                demWin.ShowDialog();

                if(!solver.Matrix.IsSolved())
                {
                    // stage 7
                    solver.RecursiveMethod();

                    demWin = new Views.DemonstrationWindow() { Title = "Stage 7. Solving sudoku, part 2" };
                    Sudoku.DrawDigits(gameFieldImg, solver.Matrix);
                    demWin.image.Source = gameFieldImg.Bitmap.ToImageSource();
                    demWin.ExplainBox.Text = $"Solved of the remaining unknown sudoku values using recursive method";
                    demWin.ShowDialog();
                }

            }
            catch (System.Exception error)
            {
                MessageBox.Show($"Some error\nSource: {error.Source}\nMessage: {error.Message}",
                                "Error",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
            }

        }
    }
}
