using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using SudokuLibrary;
using SudokuLibrary.Extensions;

namespace SudokuWPF.Views
{

    public partial class SudokuBoard : Window
    {
        public SudokuBoard()
        {
            InitializeComponent();
        }

        private void CalculateButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var cells = ReadValues();
                var solver = new SudokuSolver(cells);
                cells = solver.Calculate();

                this.Title = $"Sudoku (Time: {solver.SolvingTime.ElapsedMilliseconds}ms)";
                WriteValues(cells);                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
        }


        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            string rowNames = "ABCDEFGHI";

            for (int i = 0; i < 9; i++)
            {
                for (int k = 0; k < 9; k++)
                {
                    var cellName = rowNames[i] + (k + 1).ToString();
                    var textBox = (TextBox)this.FindName(cellName);
                    textBox.Foreground = Brushes.Black;
                    textBox.Text = "";
                }
            }
        }


        private Cell[,] ReadValues()
        {
            string rowNames = "ABCDEFGHI";

            var cell = new Cell[9,9];

            for (int i = 0; i < 9; i++)
            {
                for (int k = 0; k < 9; k++)
                {
                    var cellName = rowNames[i] + (k + 1).ToString();
                    var textBox = (TextBox)this.FindName(cellName);

                    if (textBox.Text != null && textBox.Text != "")
                    {
                        var digit = Int32.Parse(textBox.Text);

                        if (!cell.IsPossible(i, k, digit))
                        {
                            throw new InvalidOperationException($"Value in cell {cellName} is impossible");
                        }

                        cell[i, k].Value = digit;
                        cell[i, k].Preset = true;
                    }
                }
            }

            return cell;
        }

        private void WriteValues(Cell[,] cell)
        {
            string rowNames = "ABCDEFGHI";

            for (int i = 0; i < 9; i++)
            {
                for (int k = 0; k < 9; k++)
                {
                    var cellName = rowNames[i] + (k + 1).ToString();
                    var textBox = (TextBox)this.FindName(cellName);

                    if(cell[i, k].Preset)
                    {
                        textBox.Foreground = Brushes.Red;
                    }
                    else
                    {
                        textBox.Foreground = Brushes.Green;
                    }

                    textBox.Text = cell[i, k].Value.ToString();
                }
            }
        }

        private void CheckButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ReadValues();
                MessageBox.Show("It`s correct");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
        }
    }
}
