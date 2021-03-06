using System;
using System.Windows.Media.Imaging;
using System.Windows.Controls;
using System.Threading;
using System.Collections.Generic;

namespace ParcelLockers
{
    public class Human
    {
        
        protected static int ZIndexGen = 0;
        protected Thread m_Thread;
        protected Canvas m_Context;
        protected Coord m_currentPos;
        protected int m_posInQueue = 0;
        protected int m_queueId = 0;
        protected bool m_waitingInQueue = false;
        protected bool m_cameToTheParcelLocker = false;
        protected List<Uri> m_imagesUris;

        public Coord Position { get { return m_currentPos; } set { m_currentPos = value; } }
        public Image Img { get; set; }
        public int PosInQueue { get { return m_posInQueue; } set { m_posInQueue = value; } }
        public int QueueId { get { return m_queueId; } set { m_queueId = value; } }
        public bool WaitingInQueue { get { return m_waitingInQueue; } set { m_waitingInQueue = value; } }
        public bool CameToTheParcelLocker { get { return m_cameToTheParcelLocker; } set { m_cameToTheParcelLocker = value; } }
        public Thread Thread { get { return m_Thread; } }

        protected Human()
        {
            
        }
        protected void FadeOut(int imgId)
        {

            ScreenOperation.Perform(new Action(() =>
            {
                Img.Source = new BitmapImage(m_imagesUris[imgId]);
            }));

            for (int i = 0; i < 40; i++)
            {
                Thread.Sleep(2);
                ScreenOperation.Perform(new Action(() =>
                {
                    if (i == 0)
                        Img.Source = new BitmapImage(m_imagesUris[imgId]);
                    Canvas.SetLeft(Img, Canvas.GetLeft(Img) + 1);
                    m_currentPos.x++;
                    Img.Opacity -= 0.05;
                }));
            }
        }

        /*
        * public methods to move image from another thread when waiting for resource
        */

        public void MovePerson(Coord vec)
        {
            ScreenOperation.Perform(new Action(() =>
            {
                if (vec.x > 0 && vec.y == 0) // right
                {
                    Canvas.SetLeft(Img, Canvas.GetLeft(Img) + vec.x);
                    m_currentPos.x += vec.x;
                }
                else if (vec.x < 0 && vec.y == 0) // left
                {
                    Canvas.SetLeft(Img, Canvas.GetLeft(Img) + vec.x);
                    m_currentPos.x += vec.x;
                }
                else if (vec.x == 0 && vec.y > 0) // down
                {
                    Canvas.SetTop(Img, Canvas.GetTop(Img) + vec.y);
                    m_currentPos.y += vec.y;
                }
                else // up
                {
                    Canvas.SetTop(Img, Canvas.GetTop(Img) + vec.y);
                    m_currentPos.y += vec.y;
                }
            }));
        }

        protected void EnterTheQueue(int pId)
        {
            SharedResources.SafeSharedResourceOperation.WaitOne();
            m_posInQueue = SharedResources.NumPeopleInQueue[pId];
            SharedResources.PlacesTakenInQueue[pId, SharedResources.NumPeopleInQueue[pId]] = true;
            SharedResources.NumPeopleInQueue[pId]++;
            SharedResources.SafeSharedResourceOperation.ReleaseMutex();

            ScreenOperation.Perform(new Action(() =>
            {
                Canvas.SetZIndex(Img, ZIndexGen++);
            }));
            m_waitingInQueue = true;
        }

        protected void LeaveTheQueue(int pId)
        {
            SharedResources.SafeSharedResourceOperation.WaitOne();
            m_waitingInQueue = false;
            m_cameToTheParcelLocker = false;
            SharedResources.PlacesTakenInQueue[pId, 0] = false;
            SharedResources.NumPeopleInQueue[pId]--;
            SharedResources.SafeSharedResourceOperation.ReleaseMutex();
        }

        protected void ChangeImage(int imgId)
        {
            ScreenOperation.Perform(new Action(() =>
            {
                Img.Source = new BitmapImage(m_imagesUris[imgId]);
            }));
        }

    }
}
