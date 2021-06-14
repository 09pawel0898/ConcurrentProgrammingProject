using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Windows.Controls;
using System.Threading;
using System.Windows.Threading;
using System.Windows;

namespace ParcelLockers
{
    enum PersonAction
    {
        SENDING,
        PICKINGUP
    };

    class Person : Human
    {
        private static int IDGen = 0;
        private int m_Id;
        private PersonAction m_currentAction;

        public Person(Canvas context)
        {
            m_Id = IDGen++;
            Random rand = new Random();
            int randId = rand.Next(0, 6);
            m_Context = context;
            m_imagesUris = Resources.Instance.People[randId];
            InitImage();
            m_Thread = new Thread(Simulate);
            m_Thread.Start();
        }

        private void Simulate()
        {
            Random rand = new Random();
            SharedResources.Window.Dispatcher.BeginInvoke(new Action(() =>  {   Canvas.SetZIndex(Img, 200000);  }));

            while (true)
            {
                // Decide what to do in the current loop
                if (SharedResources.ParcelsShippedToPeople[m_Id].Count > 0) // if there is at least one parcel shipped to this adressee
                    m_currentAction = PersonAction.PICKINGUP;
                else 
                    m_currentAction = PersonAction.SENDING; 
                
                // two action types (sending or picking up a parcel)
                switch (m_currentAction)
                {
                    case PersonAction.SENDING:
                        Thread.Sleep(rand.Next(5000 - Defines.simulationSpeed * 450, 10000 - Defines.simulationSpeed * 700));
                        SimulateSendingParcel();    break;
                    case PersonAction.PICKINGUP:    SimulatePickingUpParcel();  break;
                }
                FadeOut(2);
                ResetPosition();
                
                // Sleep for some time
                Thread.Sleep(rand.Next(5000 - Defines.simulationSpeed * 450, 10000 - Defines.simulationSpeed * 700));
            }
        }
        private void SimulateSendingParcel()
        {
            int parcelLockerId = ParcelLocker.GetRandomParcelLockerId();
            m_queueId = parcelLockerId;
            GoToProperParcelLockerPath(parcelLockerId);
            ChangeImage(0);
            TryToQueueUpAndGetToTheParcelLocker(parcelLockerId);
        }

        private void SimulatePickingUpParcel()
        {
            int parcelLockerId = SharedResources.ParcelsShippedToPeople[m_Id].First().DestinationParcelLocker;
            m_queueId = parcelLockerId;
            GoToProperParcelLockerPath(parcelLockerId);
            ChangeImage(0);
            TryToQueueUpAndGetToTheParcelLocker(parcelLockerId);
        }

        private void GoToProperParcelLockerPath(int pId)
        {
            while (m_currentPos.x < Defines.parcelLockerPos[pId].x)
            {
                Thread.Sleep(2);
                MovePerson(new Coord(1 + (int)(Defines.simulationSpeed * 2 / 10), 0));
            }
        }

        private void TryToQueueUpAndGetToTheParcelLocker(int pId)
        {
            
            //DEBUG
            bool[,] temp = SharedResources.PlacesTakenInQueue;
            List<Human> listPeople = Simulation.m_People;

            Random rand = new Random();
            EnterTheQueue(pId);

            // threads that enter the monitor are waiting in FIFO queue
            try
            {
                QueuedLock.Enter(pId);
                //Monitor.Enter(SharedResources.ParcelLockers[pId]);
            }
            catch(Exception e)
            {
                MessageBox.Show(e.Message);
            }
            try
            {
                // waiting to get to the first position
                try
                {
                    Thread.Sleep(Timeout.Infinite);
                }
                catch (ThreadInterruptedException e) { }

                // taking selected actions on a shared resource
                //Thread.Sleep(4000);
                Thread.Sleep(rand.Next(5000 - Defines.simulationSpeed * 400, 10000 - Defines.simulationSpeed * 700));

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
                QueuedLock.Exit(pId);
                //Monitor.Exit(SharedResources.ParcelLockers[pId]);
            }
            LeaveTheQueue(pId);
        }

        private void ResetPosition()
        {
            ScreenOperation.Perform(new Action(() =>
            {
                Canvas.SetLeft(Img, Defines.sidewalkSpawnPos.x);
                Canvas.SetTop(Img, Defines.sidewalkSpawnPos.y);
                m_currentPos = new Coord(Defines.sidewalkSpawnPos.x, Defines.sidewalkSpawnPos.y);
                Img.Opacity = 1;
                Canvas.SetZIndex(Img, 200000);
            }));
        }

        private void InitImage()
        {
            ScreenOperation.Perform(new Action(() =>
            {
                Img = new Image
                {
                    Width = 80,
                    Height = 200,
                    Name = "P" + m_Id,
                    Source = new BitmapImage(m_imagesUris[2])
                };
                m_Context.Children.Add(Img);
                Canvas.SetLeft(Img, Defines.sidewalkSpawnPos.x);
                Canvas.SetTop(Img, Defines.sidewalkSpawnPos.y);
            }));
            m_currentPos = new Coord(Defines.sidewalkSpawnPos.x, Defines.sidewalkSpawnPos.y);
        }
    }
}
