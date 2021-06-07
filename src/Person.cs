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
    enum PersonAction
    {
        SENDING,
        PICKINGUP
    };

    class Person : Human
    {
        PersonAction m_currentAction;
        private static int IDGen = 0;
        protected int m_Id;

        public Person(Canvas context)
        {
            m_Id = IDGen++;

            m_Animator = new Animator(2, Resources.Instance.People);
            m_Context = context;
            m_imagesUris = Resources.Instance.People;

            SharedResources.Screen.WaitOne();
            SharedResources.Window.Dispatcher.BeginInvoke(new Action(() =>
            {
                Img = new Image
                {
                    Width = 80,
                    Height = 200,
                    Name = "P"+m_Id,
                    Source = new BitmapImage(m_imagesUris[2])
                };
                m_Context.Children.Add(Img);
                Canvas.SetLeft(Img, Defines.sidewalkSpawnPos.x);
                Canvas.SetTop(Img, Defines.sidewalkSpawnPos.y);
            }));
            SharedResources.Screen.ReleaseMutex();

            m_currentPos = new Coord(Defines.sidewalkSpawnPos.x, Defines.sidewalkSpawnPos.y);
            m_Thread = new Thread(Simulate);
            m_Thread.Start();
        }

        private void Simulate()
        {
            Random rand = new Random();

            while (true)
            {
                // Decide what to do in the current loop
                if (SharedResources.ParcelsShippedToPeople[m_Id].Count > 0)
                    m_currentAction = PersonAction.PICKINGUP;
                else
                    m_currentAction = PersonAction.SENDING; 
                
                //: PersonAction.PICKINGUP;

                // two action types (sending or picking up a parcel)
                switch (m_currentAction)
                {
                    case PersonAction.SENDING:      SimulateSendingParcel();    break;
                    case PersonAction.PICKINGUP:    SimulatePickingUpParcel();  break;
                }
                FadeOut();
                ResetPosition();
                // Sleep for some time
                Thread.Sleep(rand.Next(1000, 4000));
            }
        }
        private void SimulateSendingParcel()
        {
            int parcelLockerId = ParcelLocker.GetRandomParcelLockerId();
            //int parcelLockerId = 0;
            m_queueId = parcelLockerId;
            GoToProperParcelLockerPath(parcelLockerId);
            TryToQueueUpAndGetToTheParcelLocker(parcelLockerId);
        }

        private void SimulatePickingUpParcel()
        {
            int parcelLockerId = SharedResources.ParcelsShippedToPeople[m_Id].First().DestinationParcelLocker;
            //int parcelLockerId = 0;
            m_queueId = parcelLockerId;
            GoToProperParcelLockerPath(parcelLockerId);
            TryToQueueUpAndGetToTheParcelLocker(parcelLockerId);
        }

        private void GoToProperParcelLockerPath(int pId)
        {
            while (m_currentPos.x != Defines.parcelLockerPos[pId].x)
            {
                Thread.Sleep(2);
                MovePerson(new Coord(1,0));
            }
        }

        private void TryToQueueUpAndGetToTheParcelLocker(int pId)
        {
            EnterTheQueue(pId);

            Monitor.Enter(SharedResources.ParcelLockers[pId]);
            try
            {
                // waiting to get to the first position
                try
                {
                    Thread.Sleep(Timeout.Infinite);
                }
                catch (ThreadInterruptedException e) { }

                // taking selected actions on a shared resource
                Thread.Sleep(4000);
                switch (m_currentAction)
                {
                    case PersonAction.SENDING:
                        SharedResources.ParcelLockers[pId].PutParcelToRandomCell();
                        break;
                    case PersonAction.PICKINGUP:
                        Parcel parcelToTake = SharedResources.ParcelsShippedToPeople[m_Id].First();
                        SharedResources.ParcelLockers[pId].TakeMyParcel(parcelToTake);
                        SharedResources.ParcelsShippedToPeople[m_Id].Remove(parcelToTake);
                        break;
                }
            }
            finally
            {
                Monitor.Exit(SharedResources.ParcelLockers[pId]);
            }
            LeaveTheQueue(pId);
        }

        private void ResetPosition()
        {
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
    }
}
