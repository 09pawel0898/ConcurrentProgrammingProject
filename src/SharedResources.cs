using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Threading;
using System.Windows.Threading;

namespace ParcelLockers
{
    class SharedResources
    {
        public static Window Window;
        public static List<ParcelLocker> ParcelLockers = new List<ParcelLocker>();
        public static Mutex Screen = new Mutex();
        public static Mutex SafeSharedResourceOperation = new Mutex();
        public static int[] NumPeopleInQueue = new int[Defines.numParcelLockers];
        public static bool[,] PlacesTakenInQueue = new bool[Defines.numParcelLockers, Defines.maxPeopleInQueue];
        public static List<Parcel>[] ParcelsShippedToPeople;
    }
}
