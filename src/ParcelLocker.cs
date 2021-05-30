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
    class Cell
    {
        private static int IDGen = 0;
        private int m_Id;
        private Coord m_Pos = new Coord(0,0);
        private Canvas m_Context;
        public Image Img { get; set; }

        public Cell(Canvas context, Coord coord, Coord parcelLockerOffset)
        {
            m_Context = context;
            m_Pos = coord;
            m_Id = IDGen;
            IDGen++;

            Img = new Image
            {
                Width = 50,
                Height = 30,
                Name = "C"+m_Id,
                Source = new BitmapImage(Resources.Instance.Cells[0]),
            };
            m_Context.Children.Add(Img);
            Canvas.SetTop(Img, 1*30 + m_Pos.y*30);
            Canvas.SetLeft(Img, 1*50 + parcelLockerOffset.x + m_Pos.x*50);
        }
    }
    class ParcelLocker
    {
        private Coord m_Offset;
        private Canvas m_Context;
        private List<Cell> m_Cells;
        public ParcelLocker(Coord offset,Canvas context)
        {
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

            m_Cells[5].Img.Source = new BitmapImage(Resources.Instance.Cells[1]);
            m_Cells[6].Img.Source = new BitmapImage(Resources.Instance.Cells[1]);
            m_Cells[7].Img.Source = new BitmapImage(Resources.Instance.Cells[1]);
            m_Cells[20].Img.Source = new BitmapImage(Resources.Instance.Cells[1]);
            m_Cells[21].Img.Source = new BitmapImage(Resources.Instance.Cells[1]);
            m_Cells[37].Img.Source = new BitmapImage(Resources.Instance.Cells[1]);
            m_Cells[38].Img.Source = new BitmapImage(Resources.Instance.Cells[1]);
        }
    }
}
