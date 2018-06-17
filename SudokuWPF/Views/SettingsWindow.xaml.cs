using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using SudokuLibrary;

namespace SudokuWPF
{
    public partial class SettingsWindow : Window
    {
        SudokuLibrary.Properties.Settings setts = SudokuLibrary.Properties.Settings.Default;

        public SettingsWindow()
        {
            InitializeComponent();
            LoadDefault();
        }


        private void ApplyButton_Click(object sender, RoutedEventArgs e)
        {
            Apply();
            LoadDefault();
        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            setts.Reset();
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            var regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }



        private void LoadDefault()
        {
            MAXSIZE.Text = setts.MAXSIZE.ToString();
            RSIZE.Text = setts.RSIZE.ToString();

            TRESHOLD_MIN.Text = setts.TRESHOLD_MIN.ToString();
            TRESHOLD_MAX.Text = setts.TRESHOLD_MAX.ToString();
            L2Gradient.IsChecked = setts.L2Gradient;

            FONT.ItemsSource = Enum.GetValues(typeof(Emgu.CV.CvEnum.FontFace));
            FONT.SelectedItem = setts.FONT;

            FONTSIZE.Text = setts.FONTSIZE.ToString();
            FONTSIZEPR.Text = setts.FONTSIZEPR.ToString();

            CHAINAPPROX.ItemsSource = Enum.GetValues(typeof(Emgu.CV.CvEnum.ChainApproxMethod));
            CHAINAPPROX.SelectedItem = setts.CHAINAPPROX;

            THOCR_MIN.Text = setts.THOCR_MIN.ToString();
            THOCR_MAX.Text = setts.THOCR_MAX.ToString();
        }

        private void Apply()
        {
            setts.MAXSIZE = Int32.Parse(MAXSIZE.Text);
            setts.RSIZE = Int32.Parse(RSIZE.Text);

            setts.TRESHOLD_MIN = Int32.Parse(TRESHOLD_MIN.Text);
            setts.TRESHOLD_MAX = Int32.Parse(TRESHOLD_MAX.Text);
            setts.L2Gradient = (bool)L2Gradient.IsChecked;

            setts.FONT = (Emgu.CV.CvEnum.FontFace)FONT.SelectedItem;
            setts.FONTSIZE = Int32.Parse(FONTSIZE.Text);
            setts.FONTSIZEPR = Int32.Parse(FONTSIZEPR.Text);

            setts.CHAINAPPROX = (Emgu.CV.CvEnum.ChainApproxMethod)CHAINAPPROX.SelectedItem;

            setts.THOCR_MIN = Int32.Parse(THOCR_MIN.Text);
            setts.THOCR_MAX = Int32.Parse(THOCR_MAX.Text);

            setts.Save();
            this.Close();
        }
    }
}
