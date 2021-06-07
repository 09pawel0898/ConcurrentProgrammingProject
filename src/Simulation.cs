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
    class Simulation
    {
        Canvas m_Context = null; 
        List<Human> m_People = new List<Human>();
        Courier m_Courier = null;
        Thread m_Thread = null;
        DispatcherTimer m_Timer = null;
        int m_numPeopleInSimulation = 0;
        bool m_EndOfProgram = false;

        int m_iterCounter = 0;
        int m_nextPersonAddIter = 0;

        public Simulation(Canvas context, Window window)
        {
            m_Context = context;
            InitSharedResources(window);
            InitResources();
            InitParcelLockers();
            InitSimulationThread();
        }

        private void Run()
        {

            InitCourier();
            while (!m_EndOfProgram)
            {
                Thread.Sleep(2);
                TryAddPeopleToSimulation();
                UpdateLabels();
                UpdatePeoplePositions();
            }
        }

        /*
         * Update
         */

        private void UpdatePeoplePositions()
        {
            SharedResources.SafeSharedResourceOperation.WaitOne();
            foreach (Human person in m_People)
            {
                bool[,] tab = SharedResources.PlacesTakenInQueue;
                
                //Monitor.Enter(SharedResources.ParcelLockers[person.QueueId]);
                if (person.WaitingInQueue && !person.CameToTheParcelLocker)
                {
                    if (person.PosInQueue == 0 && person.Position.y <= Defines.parcelLockerPos[0].y )
                    {
                        person.CameToTheParcelLocker = true;
                        person.Thread.Interrupt();
                    }
                    else if(person.PosInQueue == 0)
                    {
                        person.MovePerson(new Coord(0, -1));
                    }
                    else if (!person.CameToTheParcelLocker)
                    {
                        if(person.Position.y >= Defines.parcelLockerPos[0].y + person.PosInQueue * 30)
                            person.MovePerson(new Coord(0, -1));
                        else if (SharedResources.PlacesTakenInQueue[person.QueueId, person.PosInQueue - 1] == false)
                        {
                            
                            SharedResources.PlacesTakenInQueue[person.QueueId, person.PosInQueue--] = false;
                            SharedResources.PlacesTakenInQueue[person.QueueId, person.PosInQueue] = true;
                            
                        }
                    }
                }
                //Monitor.Exit(SharedResources.ParcelLockers[person.QueueId]);
                
            }
            SharedResources.SafeSharedResourceOperation.ReleaseMutex();
        }

        private void UpdateLabels()
        {

        }

        /*
         * Initialization 
         */

        private void InitCourier()
        {
            m_People.Add(new Courier(m_Context));
        }
        private void InitSharedResources(Window window)
        {
            SharedResources.Window = window;
            for(int i = 0; i < Defines.numPeopleInSimulation; i++)
                SharedResources.ParcelsShippedToPeople[i] = new List<Parcel>();
        }

        private void InitSimulationThread()
        {
            m_Timer = new DispatcherTimer();
            m_Timer.Interval = TimeSpan.FromMilliseconds(20);
            m_Thread = new Thread(Run);
            m_Thread.Start();
        }

        private void InitResources()
        {
            Resources.AddUri(UriType.PARCELLOCKER, new Uri("/Resources/parcelLocker.png",UriKind.Relative));

            Resources.AddUri(UriType.CELL, new Uri("/Resources/cellfree.png", UriKind.Relative));
            Resources.AddUri(UriType.CELL, new Uri("/Resources/celltaken.png", UriKind.Relative));
            Resources.AddUri(UriType.CELL, new Uri("/Resources/cellcollect.png", UriKind.Relative));

            Resources.AddUri(UriType.PERSON, new Uri("/Resources/patBack1.png", UriKind.Relative));
            Resources.AddUri(UriType.PERSON, new Uri("/Resources/patLeft1.png", UriKind.Relative));
            Resources.AddUri(UriType.PERSON, new Uri("/Resources/patRight1.png", UriKind.Relative));

            Resources.AddUri(UriType.COURIER, new Uri("/Resources/matBack1.png", UriKind.Relative));
            Resources.AddUri(UriType.COURIER, new Uri("/Resources/matLeft1.png", UriKind.Relative));
            Resources.AddUri(UriType.COURIER, new Uri("/Resources/matRight1.png", UriKind.Relative));

            Resources.AddUri(UriType.COURIERCAR, new Uri("/Resources/car.png", UriKind.Relative));
            Resources.AddUri(UriType.COURIERCAR, new Uri("/Resources/car2.png", UriKind.Relative));
        }

        private void InitParcelLockers()
        {
            List<ParcelLocker> parcelLockers = new List<ParcelLocker>();
            parcelLockers.Add(new ParcelLocker(new Coord(0,0),m_Context));
            parcelLockers.Add(new ParcelLocker(new Coord(400,0),m_Context));
            parcelLockers.Add(new ParcelLocker(new Coord(800,0),m_Context));
            SharedResources.ParcelLockers = parcelLockers;
        }

        private void TryAddPeopleToSimulation()
        {
            if(m_numPeopleInSimulation < Defines.numPeopleInSimulation)
            {
                Random rand = new Random();
                if (m_nextPersonAddIter == m_iterCounter)
                {  
                    m_numPeopleInSimulation++;
                    m_nextPersonAddIter = rand.Next(200, 600);
                    m_iterCounter = 0;
                    m_People.Add(new Person(m_Context)); 
                }
                else m_iterCounter++;
            }
        }
    }
}
