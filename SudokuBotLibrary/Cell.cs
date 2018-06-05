using System.Collections.Generic;
using System.Drawing;

namespace SudokuBotLibrary
{
    struct Cell
    {
        public int Value { get; set; }

        public Rectangle Rect { get; set; }

        public List<int> Possibles { get; set; }

        public bool Preset { get; set; }

        public bool Calculated { get; set; }

        public bool IsKnown { get { return Preset || Calculated; } }
    }
}
