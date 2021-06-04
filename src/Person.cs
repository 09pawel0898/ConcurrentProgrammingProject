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
        public Person(Canvas context)
        {
            m_Animator = new Animator(2, Resources.Instance.People);
            m_Context = context;

            SharedResources.SafeSharedResourceOperation.WaitOne();
            SharedResources.Window.Dispatcher.BeginInvoke(new Action(() =>
            {
                Img = new Image
                {
                    Width = 80,
                    Height = 200,
                    Name = "P"+m_Id,
                    Source = new BitmapImage(Resources.Instance.People[4])
                };
                m_Context.Children.Add(Img);
                Canvas.SetLeft(Img, Defines.sidewalkSpawnPos.x);
                Canvas.SetTop(Img, Defines.sidewalkSpawnPos.y);
            }));
            SharedResources.SafeSharedResourceOperation.ReleaseMutex();

            m_currentPos = new Coord(Defines.sidewalkSpawnPos.x, Defines.sidewalkSpawnPos.y);
            m_Thread = new Thread(Simulate);
            m_Thread.Start();
        }

        private void Simulate()
        {
            Random rand = new Random();
            PersonAction action;

            while (true)
            {
                // Decide what to do in the current loop
                int randomNum = rand.Next(0, 100);
                action = (randomNum < 100) ?  PersonAction.SENDING : PersonAction.PICKINGUP;

                // two action types (sending or picking up a parcel)
                switch (action)
                {
                    case PersonAction.SENDING:      SimulateSendingParcel();    break;
                    case PersonAction.PICKINGUP:    SimulatePickingUpParcel();  break;
                }
                FadeOut();
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
            TryToQueueUpAndPickUpTheParcel(parcelLockerId);
        }

        private void SimulatePickingUpParcel()
        {

        }

        private void GoToProperParcelLockerPath(int pId)
        {
            while (m_currentPos.x != Defines.parcelLockerPos[pId].x)
            {
                Thread.Sleep(2);
                MovePerson(new Coord(1,0));
            }
        }

        private void TryToQueueUpAndPickUpTheParcel(int pId)
        {
            m_waitingInQueue = true;
            SharedResources.SafeSharedResourceOperation.WaitOne();
            m_posInQueue = SharedResources.NumPeopleInQueue[pId];
            
            SharedResources.PlacesTakenInQueue[pId,SharedResources.NumPeopleInQueue[pId]] = true;
            SharedResources.NumPeopleInQueue[pId]++;
            SharedResources.SafeSharedResourceOperation.ReleaseMutex();

            SharedResources.Screen.WaitOne();
            SharedResources.Window.Dispatcher.BeginInvoke(new Action(() =>
            {
                Img.Source = new BitmapImage(Resources.Instance.People[2]);
                Canvas.SetZIndex(Img, ZIndexGen++);
            }));
            SharedResources.Screen.ReleaseMutex();

            
            Monitor.Enter(SharedResources.ParcelLockers[pId]);
            try
            {
                // waiting to get to the first position
                while(!m_cameToTheParcelLocker) { }
                // taking selected actions on a shared resource
                Thread.Sleep(4000);
                SharedResources.ParcelLockers[pId].PutParcelToRandomCell();
            }
            finally { Monitor.Exit(SharedResources.ParcelLockers[pId]); }

            m_waitingInQueue = false;
            m_cameToTheParcelLocker = false;
            SharedResources.SafeSharedResourceOperation.WaitOne();
            SharedResources.PlacesTakenInQueue[pId, 0] = false;
            SharedResources.NumPeopleInQueue[pId]--;
            SharedResources.SafeSharedResourceOperation.ReleaseMutex();
        }
    }
}
