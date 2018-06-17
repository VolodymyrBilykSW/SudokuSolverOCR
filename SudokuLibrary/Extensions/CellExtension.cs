using System.Collections.Generic;

namespace SudokuLibrary.Extensions
{
    public static class CellsExtension
    {
        // Creation for each cell list of possible values.
        public static void CreatePossible(this Cell[,] matrix)
        {
            for (int yi = 0; yi < 9; yi++)
            {
                for (int xi = 0; xi < 9; xi++)
                {
                    if (!matrix[xi, yi].IsKnown)
                    {
                        for (int num = 1; num <= 9; num++)
                        {
                            if (matrix.IsPossible(xi, yi, num))
                            {
                                if (matrix[xi, yi].Possibles == null)
                                    matrix[xi, yi].Possibles = new List<int>();

                                matrix[xi, yi].Possibles.Add(num);
                            }
                        }
                    }
                }
            }
        }


        // Delete value from possibles in row, column and kvadrant.
        public static void DeleteFromPossible(this Cell[,] matrix, int constX, int constY, int num)
        {
            // delete from row
            for (int xi = 0; xi < 9; xi++)
            {
                if (!matrix[xi, constY].IsKnown)
                    matrix[xi, constY].Possibles.Remove(num);
            }

            // delete from column
            for (int yi = 0; yi < 9; yi++)
            {
                if (!matrix[constX, yi].IsKnown)
                    matrix[constX, yi].Possibles.Remove(num);
            }

            // delete from kvadrant
            int startX = constX - (constX % 3);
            int startY = constY - (constY % 3);
            for (int yi = startY; yi < startY + 3; yi++)
            {
                for (int xi = startX; xi < startX + 3; xi++)
                {
                    if (!matrix[xi, yi].IsKnown)
                        matrix[xi, yi].Possibles.Remove(num);
                }
            }
        }


        // Return true if this num don`t exist in row, column or kvadrant.
        public static bool IsPossible(this Cell[,] matrix, int constX, int constY, int num)
        {
            if (num > 9 || num < 1)
                return false;

            // check row
            for (int xi = 0; xi < 9; xi++)
            {
                if (matrix[xi, constY].Value != 0 && matrix[xi, constY].Value == num)
                    return false;
            }

            // check column
            for (int yi = 0; yi < 9; yi++)
            {
                if (matrix[constX, yi].Value != 0 && matrix[constX, yi].Value == num)
                    return false;
            }

            // check kvadrant
            int startX = constX - (constX % 3);
            int startY = constY - (constY % 3);
            for (int yi = startY; yi < startY + 3; yi++)
            {
                for (int xi = startX; xi < startX + 3; xi++)
                {
                    if (matrix[xi, yi].Value != 0 && matrix[xi, yi].Value == num)
                        return false;
                }
            }

            return true;
        }
    }
}
