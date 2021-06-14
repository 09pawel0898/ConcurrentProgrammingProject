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
    /*
     * Main simulation class, it includes all dynamic simulation entities
     */

    public class Simulation
    {
        private Thread m_Thread;
        private Canvas m_Context;
        private List<Human> m_People = new List<Human>();
        
        private int m_currentNumPeopleInSimulation = 0;
        private int m_iterCounter = 0;
        private int m_nextPersonAddIter = 0;

        private bool m_endOfProgram = false;
        private Label[,] m_label = new Label[3,3];

        public Thread Thread { get { return m_Thread; } }

        public Simulation(Canvas context, Window window)
        {
            SharedResources.Window = window;
            m_Context = context;

            InitResources();
            InitParcelLockers();
            InitSimulationThread();
            InitLabels();
        }

        private void Run()
        {
            Config();
            InitSharedResources();
            InitCourier();

            while (!m_endOfProgram)
            {
                Thread.Sleep(2);
                TryAddPeopleToSimulation();
                UpdateLabels();
                UpdatePeoplePositions();
            }
        }

        private void Config()
        {
            SharedResources.Window.Dispatcher.BeginInvoke(new Action(() =>
            {
                while (!SharedResources.Window.IsActive)
                    Thread.Sleep(1);
                ConfigWindow configWindow = new ConfigWindow();
                configWindow.Owner = SharedResources.Window;
                configWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                configWindow.Show();
                
            }));

            // Sleep until the config is over
            try
            {
                Thread.Sleep(Timeout.Infinite);
            }
            catch (ThreadInterruptedException e) { }
        }

        /*
         * People threads are sleeping when waiting on Monitor, so their animations are updated from the main loop 
         */
        private void UpdatePeoplePositions()
        {
            SharedResources.SafeSharedResourceOperation.WaitOne();

            foreach (Human human in m_People)
            {
                if (human.WaitingInQueue && !human.CameToTheParcelLocker)
                {
                    if (human.PosInQueue == 0 && human.Position.y <= Defines.parcelLockerPos[0].y )
                    {
                        human.CameToTheParcelLocker = true;
                        human.Thread.Interrupt();
                    }
                    else if(human.PosInQueue == 0)
                    {
                        human.MovePerson(new Coord(0, -(1 + (int)(Defines.simulationSpeed*2/10))));
                    }
                    else if (!human.CameToTheParcelLocker)
                    {
                        if(human.Position.y >= Defines.parcelLockerPos[0].y + human.PosInQueue * 30)
                            human.MovePerson(new Coord(0, -(1 + (int)(Defines.simulationSpeed * 2 / 10))));
                        else if (SharedResources.PlacesTakenInQueue[human.QueueId, human.PosInQueue - 1] == false)
                        { 
                            SharedResources.PlacesTakenInQueue[human.QueueId, human.PosInQueue--] = false;
                            SharedResources.PlacesTakenInQueue[human.QueueId, human.PosInQueue] = true;
                        }
                    }
                }
            }
            SharedResources.SafeSharedResourceOperation.ReleaseMutex();
        }

        private void UpdateLabels()
        {
            SharedResources.Screen.WaitOne();
            SharedResources.Window.Dispatcher.BeginInvoke(new Action(() =>
            {
                for (int i = 0; i < 3; i++)
                {
                    for (int j = 0; j < 3; j++)
                    {
                        if (j == 0)
                            m_label[i, j].Content = "People in queue : " + SharedResources.NumPeopleInQueue[i];
                        else if (j == 1)
                            m_label[i, j].Content = "Shipped parcels : " + SharedResources.ParcelLockers[i].NumShippedParcels;
                        else if (j == 2)
                            m_label[i, j].Content = "Parcels to be picked up : " + SharedResources.ParcelLockers[i].NumParcelsToPickUp;
                    }
                }
            }));
            SharedResources.Screen.ReleaseMutex();
        }

        /*
         * Initialization 
         */
        private void InitCourier()
        {
            m_People.Add(new Courier(m_Context));
        }
        private void InitSharedResources()
        {
            SharedResources.ParcelsShippedToPeople = new List<Parcel>[Defines.numPeopleInSimulation];
            SharedResources.ParcelsShippedToPeople = new List<Parcel>[Defines.numPeopleInSimulation];

            for (int i = 0; i < Defines.numPeopleInSimulation; i++)
                SharedResources.ParcelsShippedToPeople[i] = new List<Parcel>();
        }

        private void InitSimulationThread()
        {
            m_Thread = new Thread(Run);
            m_Thread.Start();
        }

        private void InitLabels()
        {
            int shiftX = 0;
            int shiftY = 0;

            SharedResources.Screen.WaitOne();
            SharedResources.Window.Dispatcher.BeginInvoke(new Action(() =>
            {
                for (int i = 0; i < 3; i++)
                {
                    for (int j = 0; j < 3; j++)
                    {
                        m_label[i, j] = new Label();
                        //m_label[i, j].FontSize = 22;

                        if (j == 0)
                            m_label[i, j].Content = "People in queue : ";
                        else if (j == 1)
                            m_label[i, j].Content = "Shipped parcels : ";
                        else if (j == 2)
                            m_label[i, j].Content = "Parcels to be picked up : ";

                        m_Context.Children.Add(m_label[i,j]);
                        Canvas.SetLeft(m_label[i, j], 10 + shiftX);
                        Canvas.SetTop(m_label[i, j], 250 + shiftY);
                        shiftY += 20;
                    }
                    shiftY = 0;
                    shiftX += 400;
                }
            }));
            SharedResources.Screen.ReleaseMutex();
        }

        private void InitResources()
        {
            for(int i = 0; i < 4; i++)
                
            Resources.AddUri(UriType.PARCELLOCKER, new Uri("/Resources/parcelLocker.png",UriKind.Relative),0);

            Resources.AddUri(UriType.CELL, new Uri("/Resources/cellfree.png", UriKind.Relative),0);
            Resources.AddUri(UriType.CELL, new Uri("/Resources/celltaken.png", UriKind.Relative),0);
            Resources.AddUri(UriType.CELL, new Uri("/Resources/cellcollect.png", UriKind.Relative),0);

            Resources.AddUri(UriType.PERSON, new Uri("/Resources/p1Back.png", UriKind.Relative),0);
            Resources.AddUri(UriType.PERSON, new Uri("/Resources/p1Left.png", UriKind.Relative),0);
            Resources.AddUri(UriType.PERSON, new Uri("/Resources/p1Right.png", UriKind.Relative),0);
            Resources.AddUri(UriType.PERSON, new Uri("/Resources/p2Back.png", UriKind.Relative), 1);
            Resources.AddUri(UriType.PERSON, new Uri("/Resources/p2Left.png", UriKind.Relative), 1);
            Resources.AddUri(UriType.PERSON, new Uri("/Resources/p2Right.png", UriKind.Relative), 1);
            Resources.AddUri(UriType.PERSON, new Uri("/Resources/p3Back.png", UriKind.Relative), 2);
            Resources.AddUri(UriType.PERSON, new Uri("/Resources/p3Left.png", UriKind.Relative), 2);
            Resources.AddUri(UriType.PERSON, new Uri("/Resources/p3Right.png", UriKind.Relative), 2);
            Resources.AddUri(UriType.PERSON, new Uri("/Resources/p4Back.png", UriKind.Relative), 3);
            Resources.AddUri(UriType.PERSON, new Uri("/Resources/p4Left.png", UriKind.Relative), 3);
            Resources.AddUri(UriType.PERSON, new Uri("/Resources/p4Right.png", UriKind.Relative), 3);

            Resources.AddUri(UriType.COURIER, new Uri("/Resources/courierBack.png", UriKind.Relative),0);
            Resources.AddUri(UriType.COURIER, new Uri("/Resources/courierLeft.png", UriKind.Relative),0);
            Resources.AddUri(UriType.COURIER, new Uri("/Resources/courierRight.png", UriKind.Relative),0);
            Resources.AddUri(UriType.COURIER, new Uri("/Resources/courierBack2.png", UriKind.Relative),0);
            Resources.AddUri(UriType.COURIER, new Uri("/Resources/courierLeft2.png", UriKind.Relative),0);
            Resources.AddUri(UriType.COURIER, new Uri("/Resources/courierRight2.png", UriKind.Relative),0);

            Resources.AddUri(UriType.COURIERCAR, new Uri("/Resources/car.png", UriKind.Relative),0);
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
            if(m_currentNumPeopleInSimulation < Defines.numPeopleInSimulation)
            {
                Random rand = new Random();
                if (m_nextPersonAddIter == m_iterCounter)
                {  
                    m_currentNumPeopleInSimulation++;
                    m_nextPersonAddIter = rand.Next(200-Defines.simulationSpeed*10, 800- Defines.simulationSpeed*35);
                    m_iterCounter = 0;
                    m_People.Add(new Person(m_Context)); 
                }
                else m_iterCounter++;
            }
        }
    }
}
