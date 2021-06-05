﻿using System;
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
        private ParcelType m_parcelType = new ParcelType();
        private int m_parcelReceiverId;

        public ParcelType Type { get { return m_parcelType; } set { m_parcelType = value; } }
        public int ParcelReceiverId { get { return m_parcelReceiverId; } set { m_parcelReceiverId = value; } }
    }

    class Cell
    {
        private static int IDGen = 0;
        private int m_Id;
        private Coord m_Pos = new Coord(0,0);
        private Canvas m_Context;
        private Parcel m_Parcel;
        public Parcel Parcel { get { return m_Parcel; } set { m_Parcel = value; } }
        public bool IsTaken { get; set; }
        public Image Img { get; set; }
        public int Id { get { return m_Id; } set { m_Id = value; } }
        public Cell(Canvas context, Coord coord, Coord parcelLockerOffset)
        {
            m_Parcel = new Parcel();
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
        private static int IDGen = 0;
        private int m_Id;
        private bool m_IsFull = false;
        private Coord m_Offset;
        private Canvas m_Context;
        private List<Cell> m_Cells;
        private int m_NumParcels;

        public int Id { get { return m_Id; } set { m_Id = value; } }
        public int NumParcels { get { return m_NumParcels; } }
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
            m_Cells[randomCellNum].Parcel.ParcelReceiverId = rand.Next(0, Defines.numPeopleInSimulation);
            m_Cells[randomCellNum].Parcel.Type = ParcelType.SENT;
            m_NumParcels++;

            SharedResources.Screen.WaitOne();
            SharedResources.Window.Dispatcher.BeginInvoke(new Action(() =>
            {
                m_Cells[randomCellNum].Img.Source = new BitmapImage(Resources.Instance.Cells[1]);
            }));
            SharedResources.Screen.ReleaseMutex();
        }
    }
}
