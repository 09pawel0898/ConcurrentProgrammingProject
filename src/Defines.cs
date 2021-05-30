using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace paczkomaty
{
    struct Coord
    {
        public int x;
        public int y;
        public Coord(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
    }

    class Defines
    {
        public static readonly int numParcelLockers = 3;
        public static readonly int maxPeopleLockers = 3;
        public static readonly int maxCouriers = 3;
        public static readonly int parcelWidth = 50;
        public static readonly int parcelHeight = 30;
        public static readonly int numCellsInRow = 7;
        public static readonly int numCellsInColumn = 6;
    }
}
