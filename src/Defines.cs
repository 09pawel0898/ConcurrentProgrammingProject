using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParcelLockers
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
        public static readonly int numPeopleInSimulation = 10;
        public static readonly int numCellsInRow = 7;
        public static readonly int numCellsInColumn = 6;
        public static readonly int cellWidth = 50;
        public static readonly int cellHeight = 30;

        public static readonly int numParcelLockers = 3;
        public static readonly int maxPeopleInQueue = 12;
        public static readonly int maxCouriers = 3;

        public static readonly int sidewalkY = 460;
        public static readonly Coord sidewalkSpawnPos = new Coord(0, 460);

        public static readonly Coord[] parcelLockerPos = new Coord[] 
        {
            new Coord(190, 80), 
            new Coord(590, 80), 
            new Coord(990, 80)
        };

        public static readonly Coord[] courierSpawnPos = new Coord[]
        {
            new Coord(310, 460),
            new Coord(710, 460),
            new Coord(1100, 460)
        };

        public static readonly Coord[] courierCarPos = new Coord[]
        {
            new Coord(10, 576),
            new Coord(400, 576),
            new Coord(790, 576)
        };

    }
}
