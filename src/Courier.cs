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
    enum CourierAction
    {
        TAKE,
        BRING
    }

    class Courier : Human
    {
        private CourierCar m_courierCar;
        private int m_currentParcelLocker;
        private List<Parcel>[] m_shippedParcelsToParcelLocker;
        private CourierAction m_currentAction;

        public Courier(Canvas context)
        {
            //m_Animator = new Animator(2, Resources.Instance.People);
            m_Context = context;
            m_imagesUris = Resources.Instance.Couriers;
            m_shippedParcelsToParcelLocker = new List<Parcel>[Defines.numParcelLockers];
            for (int i = 0; i < Defines.numParcelLockers; i++)
                m_shippedParcelsToParcelLocker[i] = new List<Parcel>();

            SharedResources.SafeSharedResourceOperation.WaitOne();
            SharedResources.Window.Dispatcher.BeginInvoke(new Action(() =>
            {
                m_courierCar = new CourierCar(m_Context);
                Img = new Image
                {
                    Width = 80,
                    Height = 200,
                    Name = "Courier",
                    Source = new BitmapImage(Resources.Instance.Couriers[1])
                };
                SetCourierOpacity(0);
                m_Context.Children.Add(Img);
                Canvas.SetLeft(Img, Defines.courierSpawnPos[0].x);
                Canvas.SetTop(Img, Defines.courierSpawnPos[0].y);
            }));
            SharedResources.SafeSharedResourceOperation.ReleaseMutex();

            m_currentPos = new Coord(Defines.courierSpawnPos[0].x, Defines.courierSpawnPos[0].y);
            m_Thread = new Thread(Simulate);
            m_Thread.Start();
        }

        private void Simulate()
        {
            Random rand = new Random();
            int parcelLockerId = 0;

            //Init simulation waiting time
            Thread.Sleep(3000);
            
            while (true)
            {
                m_currentParcelLocker = parcelLockerId;
                ResetPosition();

                Thread.Sleep(rand.Next(1000, 2000));

                while(SharedResources.ParcelLockers[m_currentParcelLocker].NumParcels < 1)
                {
                    Thread.Sleep(2);
                }

                PickUpParcels();

                Thread.Sleep(rand.Next(1000, 4000));


                if (m_shippedParcelsToParcelLocker[m_currentParcelLocker].Count > 0)
                    BringShippedParcels();

                parcelLockerId++;
                if (parcelLockerId == Defines.numParcelLockers)
                    parcelLockerId = 0;

                //drive to the next parcel locker
                m_courierCar.DriveToTheParcelLocker(parcelLockerId);
            }
        }

        private void PickUpParcels()
        {
            SetCourierOpacity(1);
            SimulatePickingUpParcels();
            FadeOut();
            ResetPosition();
            SetCourierOpacity(0);
        }

        private void BringShippedParcels()
        {
            SetCourierOpacity(1);
            SimulateBringingShippedParcels();
            //SimulatePickingUpParcels();
            FadeOut();
            ResetPosition();
            SetCourierOpacity(0);
        }

        private void SimulatePickingUpParcels()
        {
            m_queueId = m_currentParcelLocker;
            m_currentAction = CourierAction.TAKE;
            GoToProperParcelLockerPath();
            TryToQueueUpAndGetToTheParcelLocker();
        }

        private void SimulateBringingShippedParcels()
        {
            m_queueId = m_currentParcelLocker;
            m_currentAction = CourierAction.BRING;
            GoToProperParcelLockerPath();
            TryToQueueUpAndGetToTheParcelLocker();
        }

        private void GoToProperParcelLockerPath()
        {
            while (m_currentPos.x != Defines.parcelLockerPos[m_currentParcelLocker].x)
            {
                Thread.Sleep(2);
                MovePerson(new Coord(-1, 0));
            }
        }
        private void TryToQueueUpAndGetToTheParcelLocker()
        {
            EnterTheQueue(m_currentParcelLocker);

            Monitor.Enter(SharedResources.ParcelLockers[m_currentParcelLocker]);
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
                switch(m_currentAction)
                {
                    case CourierAction.TAKE:
                        TakeShippedParcels();
                        break;
                    case CourierAction.BRING:
                        PutParcelsShippedToThisParcelLocker();
                        break;
                }
                
            }
            finally
            {
                Monitor.Exit(SharedResources.ParcelLockers[m_currentParcelLocker]);
            }
            LeaveTheQueue(m_currentParcelLocker);
        }

        private void ResetPosition()
        {
            SharedResources.Screen.WaitOne();
            SharedResources.Window.Dispatcher.BeginInvoke(new Action(() =>
            {
                Img.Source = new BitmapImage(Resources.Instance.Couriers[1]);
                Canvas.SetLeft(Img, Defines.courierSpawnPos[m_currentParcelLocker].x);
                Canvas.SetTop(Img, Defines.courierSpawnPos[m_currentParcelLocker].y);
                m_currentPos = new Coord(Defines.courierSpawnPos[m_currentParcelLocker].x, Defines.courierSpawnPos[m_currentParcelLocker].y);
            }));
            SharedResources.Screen.ReleaseMutex();
        }

        private void SetCourierOpacity(double value)
        {
            SharedResources.Screen.WaitOne();
            SharedResources.Window.Dispatcher.BeginInvoke(new Action(() =>
            {
                Img.Opacity = value;
            }));
            SharedResources.Screen.ReleaseMutex();
        }

        private void TakeShippedParcels()
        {
            List<Parcel> takenParcels = SharedResources.ParcelLockers[m_currentParcelLocker].GetAllShippedParcels();
            foreach(Parcel parcel in takenParcels)
            {
                int pId = parcel.DestinationParcelLocker;
                m_shippedParcelsToParcelLocker[pId].Add(parcel);
            }
        }

        private void PutParcelsShippedToThisParcelLocker()
        {
            foreach(Parcel parcel in m_shippedParcelsToParcelLocker[m_currentParcelLocker])
            {
                SharedResources.ParcelLockers[m_currentParcelLocker].PutShippedParcelToTheParcelLocker(parcel);
            }
            m_shippedParcelsToParcelLocker[m_currentParcelLocker].Clear();
        }
    }
}
