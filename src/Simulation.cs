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
        Canvas m_Context; 
        List<Person> m_People = new List<Person>();
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
            foreach (Person person in m_People)
            {
                bool[,] tab = SharedResources.PlacesTakenInQueue;

                if (person.WaitingInQueue && !person.CameToTheParcelLocker)
                {
                    if (person.PosInQueue == 0 && person.Position.y == Defines.parcelLockerPos[0].y)
                    {
                        person.CameToTheParcelLocker = true;
                    }
                    else if(person.PosInQueue == 0)
                    {
                        person.MovePerson(new Coord(0, -1));
                    }
                    else if (!person.CameToTheParcelLocker)
                    {
                        if(person.Position.y != Defines.parcelLockerPos[0].y + person.PosInQueue * 30)
                            person.MovePerson(new Coord(0, -1));
                        else if (SharedResources.PlacesTakenInQueue[person.QueueId, person.PosInQueue - 1] == false)
                        {
                            SharedResources.PlacesTakenInQueue[person.QueueId, person.PosInQueue--] = false;
                            SharedResources.PlacesTakenInQueue[person.QueueId, person.PosInQueue] = true;
                        }
                    }
                }
            }
        }

        private void UpdateLabels()
        {

        }

        /*
         * Initialization 
         */

        private void InitSharedResources(Window window)
        {
            SharedResources.Window = window;
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

            Resources.AddUri(UriType.PERSON, new Uri("/Resources/matBack.png", UriKind.Relative));
            Resources.AddUri(UriType.PERSON, new Uri("/Resources/matBack2.png", UriKind.Relative));
            Resources.AddUri(UriType.PERSON, new Uri("/Resources/patBack1.png", UriKind.Relative));
            Resources.AddUri(UriType.PERSON, new Uri("/Resources/patLeft1.png", UriKind.Relative));
            Resources.AddUri(UriType.PERSON, new Uri("/Resources/patRight1.png", UriKind.Relative));
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
