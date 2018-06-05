using System.Diagnostics;

namespace SudokuBotLibrary
{
    class SudokuSolver
    {
        public int RecurseDeep { get; private set; } = 0;
        public Stopwatch SolvingTime { get; private set; } = new Stopwatch();

        private Cell[,] Matrix;

        public Cell[,] Calculate(Cell[,] matrix)
        {
            Matrix = matrix;

            // Creation list of possible values.
            Matrix.CreatePossible();

            bool run = true;

            SolvingTime.Start();

            // Calculation sudoky using easy methods
            while (run)
            {
                run = FindOnePossible();

                if (run == false)
                    run = FindOnlyHere();
            }

            // Callculate other values by recursive method
            RecursiveMethod();

            SolvingTime.Stop();

            return Matrix;
        }


        // Find and write cell where is only one possible value.
        private bool FindOnePossible()
        {
            bool isChanges = false;

            for (int y = 0; y < 9; y++)
            {
                for (int x = 0; x < 9; x++)
                {
                    if (!Matrix[x, y].IsKnown && Matrix[x, y].Possibles.Count == 1)
                    {
                        Matrix[x, y].Value = Matrix[x, y].Possibles[0];
                        Matrix[x, y].Calculated = true;
                        Matrix.DeleteFromPossible(x, y, Matrix[x, y].Value);

                        Matrix[x, y].Possibles.Clear();
                        isChanges = true;
                    }

                }
            }

            return isChanges;
        }

        // Find value which can be only in one cell of row, column or kvadrant.
        private bool FindOnlyHere()
        {
            bool isChanges = false;

            for (int y = 0; y < 9; y++)
            {
                for (int x = 0; x < 9; x++)
                {
                    if (!Matrix[x, y].IsKnown)
                    {
                        foreach (int value in Matrix[x, y].Possibles)
                        {
                            bool isOnlyHere = true;

                            // check row
                            for (int xi = 0; xi < 9; xi++)
                            {
                                if (!Matrix[xi, y].IsKnown && Matrix[xi, y].Possibles.Exists(s => s == value) && xi != x)
                                {
                                    isOnlyHere = false;
                                    break;
                                }
                            }

                            // check column
                            for (int yi = 0; yi < 9; yi++)
                            {
                                if (!Matrix[x, yi].IsKnown && Matrix[x, yi].Possibles.Exists(s => s == value) && yi != y)
                                {
                                    isOnlyHere = false;
                                    break;
                                }
                            }

                            // check kvadrant
                            int startX = x - (x % 3);
                            int startY = y - (y % 3);
                            for (int yi = startY; yi < startY + 3; yi++)
                            {
                                for (int xi = startX; xi < startX + 3; xi++)
                                {
                                    if (!Matrix[xi, yi].IsKnown && Matrix[xi, yi].Possibles.Exists(s => s == value) && yi != y && xi != x)
                                    {
                                        isOnlyHere = false;
                                        break;
                                    }
                                }
                            }

                            if (isOnlyHere)
                            {
                                Matrix[x, y].Value = value;
                                Matrix[x, y].Calculated = true;
                                Matrix.DeleteFromPossible(x, y, value);

                                Matrix[x, y].Possibles.Clear();
                                isChanges = true;
                                break;
                            }
                        }
                    }
                }
            }

            return isChanges;
        }

        // Try all combinations.
        private bool RecursiveMethod(int step = 0)
        {
            RecurseDeep++;

            if (step == 81)
                return true;

            int y = step / 9;
            int x = step % 9;

            if (Matrix[x, y].IsKnown)
            {
                if (RecursiveMethod(step + 1))
                    return true;
                else
                    return false;
            }

            // If cell is empty
            for (int value = 1; value < 10; value++)
            {
                // If value possible set it and go to the next step
                if (Matrix.IsPossible(x, y, value))
                {
                    Matrix[x, y].Value = value;

                    if (RecursiveMethod(step + 1))
                        return true;
                    else
                        Matrix[x, y].Value = 0;
                }
            }

            return false;
        }
    }
}
