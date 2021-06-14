using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Windows.Controls;
using System.Threading;
using System.Windows.Threading;

namespace ParcelLockers
{
    enum ParcelType
    {
        TOBEPICKEDUP,
        SENT
    }
    class Parcel
    {
        public ParcelType Type { get; set; }
        public int ParcelReceiverId { get; set; }
        public int DestinationParcelLocker { get; set; }
    }

    class Cell
    {
        private static int IDGen = 0;
        public Parcel Parcel { get; set; }
        public bool IsTaken { get; set; }
        public Image Img { get; set; }
        public int Id { get; set; }
        public Cell(Canvas context, Coord coord, Coord parcelLockerOffset)
        {
            Parcel = new Parcel();
            Id = IDGen;
            IDGen++;

            Img = new Image
            {
                Width = 50,
                Height = 30,
                Name = "C"+Id,
                Source = new BitmapImage(Resources.Instance.Cells[0]),
            };
            context.Children.Add(Img);
            Canvas.SetTop(Img, 1*30 + coord.y*30);
            Canvas.SetLeft(Img, 1*50 + parcelLockerOffset.x + coord.x*50);
        }
    }
    class ParcelLocker
    {
        private static int IDGen = 0;
        private int m_Id;
        private bool m_IsFull = false;
        private Coord m_Offset;
        private Canvas m_Context;
        private List<Cell> m_Cells;
        private int m_numShippedParcels;
        private int m_numParcelsToPickUp;

        public int Id { get { return m_Id; } set { m_Id = value; } }
        public int NumShippedParcels { get { return m_numShippedParcels; } }
        public int NumParcelsToPickUp { get { return m_numParcelsToPickUp; } }
        public ParcelLocker(Coord offset,Canvas context)
        {
            m_Id = IDGen;
            IDGen++;
            m_Context = context;
            m_Offset = offset;
            m_Cells = new List<Cell>();
            InitParcelLockerCells();
        }

        private void InitParcelLockerCells()
        {
            for(int i = 0; i < Defines.numCellsInRow; i++)
            {
                for(int j = 0; j < Defines.numCellsInColumn; j++)
                {
                    if(i != 3 || !(j==3 || j==4 || j==5))
                        m_Cells.Add(new Cell(m_Context,new Coord(i,j),m_Offset));
                }
            }
        }

        public static int GetRandomParcelLockerId()
        {
            Random rand = new Random();
            int result;
            do
            {
                result = rand.Next(0, Defines.numParcelLockers);
            } while (SharedResources.ParcelLockers[result].m_IsFull == true);
            return result;
        }

        public void PutParcelToRandomCell()
        {
            Random rand = new Random();
            int randomCellNum;

            do
            { 
                randomCellNum = rand.Next(0, m_Cells.Count);
            } 
            while (m_Cells[randomCellNum].IsTaken);

            m_Cells[randomCellNum].IsTaken = true;
            m_Cells[randomCellNum].Parcel.DestinationParcelLocker = rand.Next(0, Defines.numParcelLockers);
            m_Cells[randomCellNum].Parcel.ParcelReceiverId = rand.Next(0, Defines.numPeopleInSimulation);
            m_Cells[randomCellNum].Parcel.Type = ParcelType.SENT;
            m_numShippedParcels++;

            ScreenOperation.Perform(new Action(() =>
            {
                m_Cells[randomCellNum].Img.Source = new BitmapImage(Resources.Instance.Cells[1]);
            }));
        }

        public List<Parcel> GetAllShippedParcels()
        {
            List<Parcel> parcelList = new List<Parcel>();

            foreach (Cell cell in m_Cells)
            {
                if(cell.IsTaken && cell.Parcel.Type == ParcelType.SENT)
                {
                    cell.Parcel.Type = ParcelType.TOBEPICKEDUP;
                    parcelList.Add(cell.Parcel);
                    cell.IsTaken = false;
                    m_numShippedParcels--;

                    ScreenOperation.Perform(new Action(() =>
                    {
                        cell.Img.Source = new BitmapImage(Resources.Instance.Cells[0]);
                    }));
                }
            }
            return parcelList;
        }

        public void PutShippedParcelToTheParcelLocker(Parcel shippedParcel)
        {
            Random rand = new Random();
            int randomCellNum;

            do
            {
                randomCellNum = rand.Next(0, m_Cells.Count);
            }
            while (m_Cells[randomCellNum].IsTaken);

            m_Cells[randomCellNum].IsTaken = true;
            m_Cells[randomCellNum].Parcel = shippedParcel;
            m_numParcelsToPickUp++;

            ScreenOperation.Perform(new Action(() =>
            {
                m_Cells[randomCellNum].Img.Source = new BitmapImage(Resources.Instance.Cells[2]);
            }));

            // add the package to the list of packages sent to this addressee
            SharedResources.ParcelsShippedToPeople[shippedParcel.ParcelReceiverId].Add(shippedParcel);
        }

        public void TakeMyParcel(Parcel parcelToTake)
        {
            foreach (Cell cell in m_Cells)
            {
                if (cell.IsTaken && cell.Parcel == parcelToTake && cell.Parcel.Type == ParcelType.TOBEPICKEDUP)
                {
                    cell.IsTaken = false;
                    m_numParcelsToPickUp--;
                    ScreenOperation.Perform(new Action(() =>
                    {
                        cell.Img.Source = new BitmapImage(Resources.Instance.Cells[0]);
                    }));
                    return;
                }
            }
        }

        /*
        public static int GetLastFreePositionInQueue(int queueId)
        {
            int result = 0;
            for(int i = 0; i < Defines.maxPeopleInQueue; i++)
            {
                if (SharedResources.PlacesTakenInQueue[queueId, i] == false)
                {
                    result = i;
                    return result;
                }
            }
            return -1;
        }
       */
    }
}
