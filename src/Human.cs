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
    class Human
    {
        protected static int IDGen = 0;
        protected static int ZIndexGen = 0;
        protected Thread m_Thread;
        protected Canvas m_Context;
        protected Animator m_Animator;
        protected int m_Id;
        protected Coord m_currentPos;
        protected int m_posInQueue = 0;
        protected int m_queueId = 0;
        protected bool m_waitingInQueue = false;
        protected bool m_cameToTheParcelLocker = false;

        public Coord Position { get { return m_currentPos; } set { m_currentPos = value; } }
        public Image Img { get; set; }
        public int PosInQueue { get { return m_posInQueue; } set { m_posInQueue = value; } }
        public int QueueId { get { return m_queueId; } set { m_queueId = value; } }
        public bool WaitingInQueue { get { return m_waitingInQueue; } set { m_waitingInQueue = value; } }
        public bool CameToTheParcelLocker { get { return m_cameToTheParcelLocker; } set { m_cameToTheParcelLocker = value; } }

        protected Human()
        {
            m_Id = IDGen++;
        }
        protected void FadeOut()
        {

            SharedResources.Screen.WaitOne();
            SharedResources.Window.Dispatcher.BeginInvoke(new Action(() =>
            {
                Img.Source = new BitmapImage(Resources.Instance.People[4]);
            }));
            SharedResources.Screen.ReleaseMutex();

            for (int i = 0; i < 40; i++)
            {
                Thread.Sleep(2);
                SharedResources.Screen.WaitOne();
                SharedResources.Window.Dispatcher.BeginInvoke(new Action(() =>
                {
                    if (i == 0)
                        Img.Source = new BitmapImage(Resources.Instance.People[4]);
                    Canvas.SetLeft(Img, Canvas.GetLeft(Img) + 1);
                    m_currentPos.x++;
                    Img.Opacity -= 0.05;
                }));
                SharedResources.Screen.ReleaseMutex();
            }

            SharedResources.Screen.WaitOne();
            SharedResources.Window.Dispatcher.BeginInvoke(new Action(() =>
            {
                Canvas.SetLeft(Img, Defines.sidewalkSpawnPos.x);
                Canvas.SetTop(Img, Defines.sidewalkSpawnPos.y);
                m_currentPos = new Coord(Defines.sidewalkSpawnPos.x, Defines.sidewalkSpawnPos.y);
                Img.Opacity = 1;
            }));
            SharedResources.Screen.ReleaseMutex();
        }

        /*
        * public methods to move image from another thread when waiting for resource
        */

        public void MovePerson(Coord vec)
        {
            SharedResources.Screen.WaitOne();
            SharedResources.Window.Dispatcher.BeginInvoke(new Action(() =>
            {
                if (vec.x > 0 && vec.y == 0) // right
                {
                    Canvas.SetLeft(Img, Canvas.GetLeft(Img) + vec.x);
                    m_currentPos.x++;
                }
                else if (vec.x < 0 && vec.y == 0) // left
                {
                    Canvas.SetLeft(Img, Canvas.GetLeft(Img) - vec.x);
                    m_currentPos.x--;
                }
                else if (vec.x == 0 && vec.y > 0) // down
                {
                    Canvas.SetTop(Img, Canvas.GetTop(Img) + vec.y);
                    m_currentPos.y++;
                }
                else // up
                {
                    Canvas.SetTop(Img, Canvas.GetTop(Img) + vec.y);
                    m_currentPos.y--;
                }

            }));
            SharedResources.Screen.ReleaseMutex();
        }
    }
}
