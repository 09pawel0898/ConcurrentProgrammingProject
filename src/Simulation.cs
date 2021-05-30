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
        List<ParcelLocker> m_ParcelLockers = new List<ParcelLocker>();
        List<Person> m_People = new List<Person>();
        Thread m_Thread = null;
        DispatcherTimer timer = null;

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

            SharedResources.Window.Dispatcher.BeginInvoke(new Action(() =>
            {
                m_People.Add(new Person(m_Context));
            }));

            
            while(true)
            {
                Thread.Sleep(2);
                UpdateLabels();
            }
        }

        private void InitSharedResources(Window window)
        {
            SharedResources.Window = window;
        }

        private void UpdateLabels()
        {

        }

        private void InitSimulationThread()
        {
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(20);
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
        }

        private void InitParcelLockers()
        {
            m_ParcelLockers.Add(new ParcelLocker(new Coord(0,0),m_Context));
            m_ParcelLockers.Add(new ParcelLocker(new Coord(400,0),m_Context));
            m_ParcelLockers.Add(new ParcelLocker(new Coord(800,0),m_Context));
        }
    }
}
