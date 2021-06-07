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
    class CourierCar
    {
        private Canvas m_Context;
        private Coord m_Position;

        public Coord Position { get { return m_Position; } set { m_Position = value; } }
        public Image Img { get; set; }

        public CourierCar(Canvas context)
        {
            m_Context = context;
            Img = new Image
            {
                Width = 450,
                Height = 200,
                Name = "Car",
                Source = new BitmapImage(Resources.Instance.CourierCars[0])
            };
            m_Context.Children.Add(Img);
            Canvas.SetZIndex(Img, 2147483646);
            Canvas.SetLeft(Img, Defines.courierCarPos[0].x);
            Canvas.SetTop(Img, Defines.courierCarPos[0].y);
            m_Position = new Coord(Defines.courierCarPos[0].x, Defines.courierCarPos[0].y);
        }

        public void DriveToTheParcelLocker(int pId)
        {

            if (Defines.parcelLockerPos[pId].x > m_Position.x)
            {
                while (m_Position.x != Defines.courierCarPos[pId].x)
                {
                    Thread.Sleep(2);
                    MoveRight();
                }
            }
            else
            {
                while (m_Position.x != Defines.courierCarPos[Defines.numParcelLockers - 1].x + 400)
                {
                    Thread.Sleep(2);
                    MoveRight();
                }

                SharedResources.Screen.WaitOne();
                SharedResources.Window.Dispatcher.BeginInvoke(new Action(() =>
                {
                    Canvas.SetLeft(Img, Defines.courierCarPos[0].x -400);
                    m_Position.x = Defines.courierCarPos[0].x - 400;
                }));
                SharedResources.Screen.ReleaseMutex();

                while (m_Position.x != Defines.courierCarPos[pId].x)
                {
                    Thread.Sleep(2);
                    MoveRight();
                }
            }
        }

        private void MoveRight()
        {
            SharedResources.Screen.WaitOne();
            SharedResources.Window.Dispatcher.BeginInvoke(new Action(() =>
            {
                Canvas.SetLeft(Img, Canvas.GetLeft(Img) + 1);
                m_Position.x++;
            }));
            SharedResources.Screen.ReleaseMutex();
        }
    }
}
